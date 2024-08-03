using Fusion;
using FusionHelpers;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using TMPro;

namespace Agit.FortressCraft
{
	/// <summary>
	/// The Player class represent the players avatar - in this case the Tank.
	/// </summary>
	[RequireComponent(typeof(NetworkCharacterController))]
	public class Player : FusionPlayer
	{
        public enum Stage
        {
            New,
            TeleportOut,
            TeleportIn,
            Active,
            Dead
        }

        private const int MAX_LIVES = 100;
		private const int MAX_HEALTH = 1000;

		//public float HP { get; set; }
		public SpawnCastle _spawnCastle;
		[SerializeField] private Transform _commander;
		[SerializeField] private TankTeleportInEffect _teleportIn;
		[SerializeField] private TankTeleportOutEffect _teleportOutPrefab;
		[SerializeField] private float _respawnTime;

		public struct DamageEvent : INetworkEvent
		{
			public Vector3 impulse;
			public int damage;
		}
		
		public struct PickupEvent : INetworkEvent
		{
			public int powerup;
		}

		[Networked] public Stage stage { get; set; }
		[Networked] public float life { get; set; }
		[Networked] private TickTimer respawnTimer { get; set; }
		[Networked] private TickTimer invulnerabilityTimer { get; set; }
		[Networked] public int lives { get; set; }
		[Networked] public bool ready { get; set; }

		public bool isActivated => (gameObject.activeInHierarchy && (stage == Stage.Active || stage == Stage.TeleportIn));
		public bool isRespawningDone => stage == Stage.TeleportIn && respawnTimer.Expired(Runner);

		public Vector3 velocity => Object != null && Object.IsValid ? _cc.Velocity : Vector3.zero;
		public Vector3 commanderPosition => _commander.position;

		public Quaternion commanderRotation => _commander.rotation;

		public GameObject cameraTarget => _cc.gameObject;

		private NetworkCharacterController _cc;
		private Collider _collider;
		private float _respawnInSeconds = -1;

		private ChangeDetector changes;

		private NetworkInputData _oldInput;

		private NetworkMecanimAnimator _netAnimator;
		private AnimatorStateInfo animState;
		private readonly static int animAttack = Animator.StringToHash("Base Layer.AttackState");
		//public Animator anim;
		private TickTimer attackInputTimer;
		private Vector2 lastDir = Vector2.left;
		private ArcherFire archerFire;
		private ArrowVector arrowVector;
		private CommanderBodyCollider bodyCollider;
		private bool died = false;

		public string OwnType { get; set; }

		// Hit Info
		List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

        [Networked] public NetworkString<_32> PlayerName { get; set; }
        [SerializeField] TextMeshPro playerNameLabel;

        [Networked] public NetworkString<_256> LastPublicChat { get; set; }
        string currentChat = "";

		[Networked] public bool IsDestroyCastle { get; set; }

		public bool isBuildCastle;

		[SerializeField] private GameObject _commanderRoot;
		[SerializeField] private GameObject _commanderRootDefault;

        public void ToggleReady()
		{
			ready = !ready;
		}

		public void ResetReady()
		{
			ready = false;
		}

		private void Awake()
		{
			_cc = GetComponent<NetworkCharacterController>();
			_collider = GetComponentInChildren<Collider>();
			_netAnimator = GetComponent<NetworkMecanimAnimator>();
			archerFire = GetComponentInChildren<ArcherFire>();
			arrowVector = GetComponentInChildren<ArrowVector>();
			bodyCollider = GetComponentInChildren<CommanderBodyCollider>();
		}

		public override void InitNetworkState()
		{
			stage = Stage.New;
			lives = MAX_LIVES;
			life = MAX_HEALTH;
			IsDestroyCastle = false;
        }

		public override void Spawned()
		{
			base.Spawned();

			DontDestroyOnLoad(gameObject);

			changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

			ready = false;

			// Proxies may not be in state "NEW" when they spawn, so make sure we handle the state properly, regardless of what it is
			OnStageChanged();

			_respawnInSeconds = 0;

			RegisterEventListener((DamageEvent evt) => ApplyAreaDamage(evt.impulse, evt.damage));
			RegisterEventListener((PickupEvent evt) => OnPickup(evt));

			// PlayerName Change
			var fusionLauncher = FindObjectOfType<FusionLauncher>();

			if (fusionLauncher != null)
			{
				PlayerName = new NetworkString<_32>(fusionLauncher.playerName);
			}

			ChatSystem.instance.playerName = fusionLauncher.playerName;


			switch (PlayerIndex)
			{
				case 0:
					OwnType = "A";
					break;
				case 1:
					OwnType = "B";
					break;
				case 2:
					OwnType = "C";
					break;
				case 3:
					OwnType = "D";
					break;
			}

			transform.Find("UnitRoot").gameObject.tag = "Unit_" + OwnType;
			archerFire.OwnType = OwnType;

			attackInputTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
		{
			Debug.Log($"Despawned PlayerAvatar for PlayerRef {PlayerId}");
			base.Despawned(runner, hasState);
			SpawnTeleportOutFx();
		}

		private void OnPickup(PickupEvent evt)
		{
			PowerupElement powerup = PowerupSpawner.GetPowerup(evt.powerup);

			if (powerup.powerupType == PowerupType.HEALTH)
				life = MAX_HEALTH;
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCSetScale(Vector3 scale)
        {
			transform.localScale = scale;
        }

		public override void FixedUpdateNetwork()
		{
			if (Object.HasStateAuthority)
			{
				CheckRespawn();

				if (isRespawningDone)
					ResetPlayer();
			}

			if (died) return;
			animState = _netAnimator.Animator.GetCurrentAnimatorStateInfo(0);
			if (InputController.fetchInput)
			{
                if (GetInput(out NetworkInputData input))
				{
					if( lastDir.x > 0.0f )
                    {
						RPCSetScale(new Vector3(-1 * Mathf.Abs(transform.localScale.x),
									transform.localScale.y, transform.localScale.z));
                    }
					else
                    {
						RPCSetScale(new Vector3(Mathf.Abs(transform.localScale.x),
									transform.localScale.y, transform.localScale.z));
					}

					if( animState.fullPathHash != animAttack )
                    {
						MovePlayer(input.moveDirection.normalized, input.aimDirection.normalized);
					}

					if(input.moveDirection.normalized != Vector2.zero)
                    {
						lastDir = input.moveDirection.normalized;
					}

					if( arrowVector != null )
                    {
						arrowVector.TargetDirection = lastDir;
					}

					if (input.IsDown(NetworkInputData.BUTTON_FIRE_PRIMARY))
					{
						if( attackInputTimer.Expired(Runner) )
                        {
							archerFire.FireDirection = lastDir;
							Debug.Log(lastDir);
							_netAnimator.Animator.SetTrigger("Attack");
							attackInputTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
						}
                    }
					else
                    {
						if( input.moveDirection.normalized != Vector2.zero )
                        {
							_netAnimator.Animator.SetBool("isMove", true);
						}
						else
                        {
							_netAnimator.Animator.SetBool("isMove", false);
						}
                    }

					if (isBuildCastle && Object.HasStateAuthority && input.WasPressed(NetworkInputData.BUTTON_TOGGLE_SPAWNCASTLE, _oldInput))
						_spawnCastle.SpawnCastleObject();

                    _oldInput = input;
				}
			}
			/*
			if (Object.HasStateAuthority)
			{
				CheckRespawn();

				if (isRespawningDone)
					ResetPlayer();
			}
			*/
		}

        /// <summary>
        /// Render is the Fusion equivalent of Unity's Update() and unlike FixedUpdateNetwork which is very different from FixedUpdate,
        /// Render is in fact exactly the same. It even uses the same Time.deltaTime time steps. The purpose of Render is that
        /// it is always called *after* FixedUpdateNetwork - so to be safe you should use Render over Update if you're on a
        /// SimulationBehaviour.
        ///
        /// Here, we use Render to update visual aspects of the Tank that does not involve changing of networked properties.
        /// </summary>
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(stage):
                        OnStageChanged();
                        break;
                    case nameof(PlayerName):
                        OnPlayerNameChanged();
                        break;
                    case nameof(LastPublicChat):
                        OnChatChanged();
							break;
					case nameof(IsDestroyCastle):
                        break;
                }
            }

            var interpolated = new NetworkBehaviourBufferInterpolator(this);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SetDestroyCastle(PlayerRef playerRef)
		{
			if (PlayerId == playerRef && HasStateAuthority)
			{
                IsDestroyCastle = true;
                GameObject.Find("UIManager").GetComponent<UIManager>().OnDefeatPanel();
            }
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_Winner(PlayerRef playerRef)
        {
            if (PlayerId == playerRef && HasStateAuthority)
            {
                GameObject.Find("UIManager").GetComponent<UIManager>().OnVictoryPanel();
            }
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_CommanderJobChanger()
        {
			if (!HasStateAuthority) return;

			_commanderRootDefault.SetActive(false);
			_commanderRoot.SetActive(true);
        }


        public void OnPlayerNameChanged()
        {
            playerNameLabel.text = PlayerName.ToString();
        }

        public void ChatGate()
        {
            currentChat = ChatSystem.instance.chatInputField.text;
            LastPublicChat = new NetworkString<_256>(currentChat);
            currentChat = "";
        }

        public void OnChatChanged()
        {
            ChatSystem.instance.chatDisplay.text += "\n" + PlayerName.ToString() + " : " + LastPublicChat.ToString();
            ChatSystem.instance.chatInputField.text = "";
        }


        /// <summary>
        /// Apply damage to Tank with an associated impact impulse
        /// </summary>
        /// <param name="impulse"></param>
        /// <param name="damage"></param>
        /// <param name="attacker"></param>
        public void ApplyAreaDamage(Vector3 impulse, int damage)
		{
			if (!isActivated)
				return;

			if (Runner.TryGetSingleton(out GameManager gameManager))
			{
				if (damage >= life)
				{
					life = 0;
					stage = Stage.Dead;

					if (gameManager.currentPlayState == GameManager.PlayState.LEVEL)
						lives -= 1;

					if (lives > 0)
						Respawn(_respawnTime);
				}
				else
				{
					life -= (byte)damage;
					Debug.Log($"Player {PlayerId} took {damage} damage, life = {life}");
				}
			}
		}

		private void MovePlayer(Vector2 moveVector, Vector2 aimVector)
		{
			if (!isActivated)
				return;

			_cc.Move(new Vector3(moveVector.x, moveVector.y, 0));

			if (aimVector.sqrMagnitude > 0)
				_commander.forward = new Vector3(aimVector.x, 0, aimVector.y);
		}

		public void Reset()
		{
			Debug.Log($"Resetting player #{PlayerIndex} ID:{PlayerId}");
			ready = false;
			lives = MAX_LIVES;
		}

		public void Respawn( float inSeconds = 0 )
		{
			_respawnInSeconds = inSeconds;
		}

		private void CheckRespawn()
		{
			if (_respawnInSeconds >= 0)
			{
				Debug.Log("Respawn Time: " + _respawnInSeconds);
				_respawnInSeconds -= Runner.DeltaTime;

				if (_respawnInSeconds <= 0)
				{
					_netAnimator.Animator.SetTrigger("Idle");
					died = false;
					SpawnPoint spawnpt = Runner.GetLevelManager().GetPlayerSpawnPoint( PlayerIndex );

					if (spawnpt == null)
					{
						_respawnInSeconds = Runner.DeltaTime;
						Debug.LogWarning($"No Spawn Point for player #{PlayerIndex} ID:{PlayerId} - trying again in {_respawnInSeconds} seconds");
						return;
					}

					Debug.Log($"Respawning Player #{PlayerIndex} ID:{PlayerId}, life={life}, lives={lives}, hasStateAuth={Object.HasStateAuthority} from state={stage} @{spawnpt}");

					// Make sure we don't get in here again, even if we hit exactly zero
					_respawnInSeconds = -1;

					// Restore health
					life = MAX_HEALTH;

					// Start the respawn timer and trigger the teleport in effect
					respawnTimer = TickTimer.CreateFromSeconds(Runner, 1);
					invulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, 1);

					// Place the tank at its spawn point. This has to be done in FUN() because the transform gets reset otherwise
					Transform spawn = spawnpt.transform;
					_cc.Teleport( spawn.position, spawn.rotation );

					// If the player was already here when we joined, it might already be active, in which case we don't want to trigger any spawn FX, so just leave it ACTIVE
					if (stage != Stage.Active)
						stage = Stage.TeleportIn;

					Debug.Log($"Respawned player {PlayerId} @ {spawn.position}, tick={Runner.Tick}, timer={respawnTimer.IsRunning}:{respawnTimer.TargetTick}, life={life}, lives={lives}, hasStateAuth={Object.HasStateAuthority} to state={stage}");
				}
			}
		}

		public void OnStageChanged()
		{
			switch (stage)
			{
				case Stage.TeleportIn:
					Debug.Log($"Starting teleport for player {PlayerId} @ {transform.position} cc@ {_cc.Data.Position}, tick={Runner.Tick}");
					break;
				case Stage.Active:
					break;
				case Stage.Dead:
					if(Runner.TryGetSingleton( out GameManager gameManager))
						gameManager.OnCommanderDeath();
					break;
				case Stage.TeleportOut:
					SpawnTeleportOutFx();
					break;
			}
			_collider.enabled = stage != Stage.Dead;
		}

		private void SpawnTeleportOutFx()
		{
			// TankTeleportOutEffect teleout = LocalObjectPool.Acquire(_teleportOutPrefab, transform.position, transform.rotation, null);
		}

		private void ResetPlayer()
		{
			Debug.Log($"Resetting player {PlayerId}, tick={Runner.Tick}, timer={respawnTimer.IsRunning}:{respawnTimer.TargetTick}, life={life}, lives={lives}, hasStateAuth={Object.HasStateAuthority} to state={stage}");
			stage = Stage.Active;
		}

		public void TeleportOut()
		{
			if (stage == Stage.Dead || stage==Stage.TeleportOut)
				return;

			if (Object.HasStateAuthority)
				stage = Stage.TeleportOut;
		}

		public void CheckDamaged()
        {
			if( bodyCollider.Damaged > 0.0f )
            {
				life -= bodyCollider.Damaged;
				bodyCollider.Damaged = 0.0f;

				if( life <= 0.0f )
                {
					Die();
                }
            }
        }

		public void Die()
        {
			died = true;
			_netAnimator.Animator.SetTrigger("Death");
			//stage = Stage.Dead;
			Respawn(5);
        }

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCCheckDamaged()
        {
			CheckDamaged();
        }
	}
}
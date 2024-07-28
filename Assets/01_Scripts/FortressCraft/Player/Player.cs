using Fusion;
using FusionHelpers;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
<<<<<<< HEAD
using TMPro;
=======
>>>>>>> Seong_0.01

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

<<<<<<< HEAD
        private const int MAX_LIVES = 100;
=======
		public enum PlayerClass
		{
			Warrior,
			Archer,
			Mage,
			Rogue
		}

        private const int MAX_LIVES = 3;
>>>>>>> Seong_0.01
		private const int MAX_HEALTH = 100;

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
		[Networked] private int life { get; set; }
		[Networked] private TickTimer respawnTimer { get; set; }
		[Networked] private TickTimer invulnerabilityTimer { get; set; }
		[Networked] public int lives { get; set; }
		[Networked] public bool ready { get; set; }
<<<<<<< HEAD
=======
		
>>>>>>> Seong_0.01

		public bool isActivated => (gameObject.activeInHierarchy && (stage == Stage.Active || stage == Stage.TeleportIn));
		public bool isRespawningDone => stage == Stage.TeleportIn && respawnTimer.Expired(Runner);

		public Vector3 velocity => Object != null && Object.IsValid ? _cc.Velocity : Vector3.zero;
		public Vector3 commanderPosition => _commander.position;

		public Quaternion commanderRotation => _commander.rotation;

		public GameObject cameraTarget => _cc.gameObject;

		private NetworkCharacterController _cc;
		private Collider _collider;
		private float _respawnInSeconds = -1;
<<<<<<< HEAD
=======

		private ChangeDetector _changes;

		private NetworkInputData _oldInput;
		public GameObject camera;
		public CinemachineVirtualCamera vCam;
		public Animator anim;
		public GameObject weapon;
		public BoxCollider2D weaponCollider;
		public PlayerClass playerClass;
		public PlayerClass currentClass;
		// Hit Info
		List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

>>>>>>> Seong_0.01

		private ChangeDetector changes;

		private NetworkInputData _oldInput;
		

		public Animator anim;

		// Hit Info
		List<LagCompensatedHit> hits = new List<LagCompensatedHit>();


        [Networked] public NetworkString<_32> PlayerName { get; set; }
        [SerializeField] TextMeshPro playerNameLabel;

        [Networked] public NetworkString<_256> LastPublicChat { get; set; }
        string currentChat = "";

		[Networked] public bool IsDestroyCastle { get; set; }

		public GameObject IN_GAME_Panel;
		public GameObject Defeat_Panel;

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

			weapon = GetComponentInChildren<PlayerWeapon>().gameObject;
			weaponCollider = weapon.GetComponent<BoxCollider2D>();

			camera = GameObject.Find("Virtual Camera");
			vCam = camera.GetComponent<CinemachineVirtualCamera>();
			vCam.Follow = this.gameObject.transform;
			weaponCollider.enabled = false;
			//TEST...
			currentClass = PlayerClass.Warrior;

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

<<<<<<< HEAD
=======

>>>>>>> Seong_0.01
			// Proxies may not be in state "NEW" when they spawn, so make sure we handle the state properly, regardless of what it is
			OnStageChanged();

			_respawnInSeconds = 0;
			
			RegisterEventListener( (DamageEvent evt) => ApplyAreaDamage(evt.impulse, evt.damage) );
			RegisterEventListener( (PickupEvent evt) => OnPickup(evt));

            // PlayerName Change

            var fusionLauncher = FindObjectOfType<FusionLauncher>();

            if (fusionLauncher != null)
            {
                PlayerName = new NetworkString<_32>(fusionLauncher.playerName);
            }

            ChatSystem.instance.playerName = fusionLauncher.playerName;
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

		public override void FixedUpdateNetwork()
		{
			if (InputController.fetchInput)
			{
                if (GetInput(out NetworkInputData input))
				{
<<<<<<< HEAD
                    MovePlayer(input.moveDirection.normalized, input.aimDirection.normalized);

					if (input.IsDown(NetworkInputData.BUTTON_FIRE_PRIMARY))
					{
						anim.SetTrigger("Attack");
                    }

=======

                    MovePlayer(input.moveDirection.normalized, input.aimDirection.normalized);

					if (input.IsDown(NetworkInputData.BUTTON_FIRE_PRIMARY))
					{
						Attack();
						anim.SetTrigger("Attack");
                    }

					if(input.IsDown(NetworkInputData.BUTTON_FIRE_SECONDARY))
					{
						Skill01();
						anim.SetTrigger("Attack");
					}

					if(input.IsDown(NetworkInputData.BUTTON_FIRE_TERTIARY))
					{
						Skill02();
						anim.SetTrigger("Attack");
					}

					if(input.IsDown(NetworkInputData.BUTTON_FIRE_FORTH))
					{
						Skill03();
						anim.SetTrigger("Attack");
					}

>>>>>>> Seong_0.01
                    if (Object.HasStateAuthority && input.WasPressed(NetworkInputData.BUTTON_TOGGLE_READY, _oldInput))
						ToggleReady();

					_oldInput = input;
				}
			}

			if (Object.HasStateAuthority)
			{
				CheckRespawn();

				if (isRespawningDone)
					ResetPlayer();
			}
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
                        Debug.Log("Render");
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

<<<<<<< HEAD
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
            ChatSystem.instance.chatDisplay.text += PlayerName.ToString() + " :" + LastPublicChat.ToString() + "\n";
            ChatSystem.instance.chatInputField.text = "";
        }


        /// <summary>
        /// Apply damage to Tank with an associated impact impulse
        /// </summary>
        /// <param name="impulse"></param>
        /// <param name="damage"></param>
        /// <param name="attacker"></param>
        public void ApplyAreaDamage(Vector3 impulse, int damage)
=======
		
		/// <summary>
		/// Apply damage to Tank with an associated impact impulse
		/// </summary>
		/// <param name="impulse"></param>
		/// <param name="damage"></param>
		/// <param name="attacker"></param>
		public void ApplyAreaDamage(Vector3 impulse, int damage)
>>>>>>> Seong_0.01
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
<<<<<<< HEAD
=======
		}

		private void MovePlayer(Vector2 moveVector, Vector2 aimVector)
		{
			if (!isActivated)
				return;

			_cc.Move(new Vector3(moveVector.x, moveVector.y, 0));

			if (aimVector.sqrMagnitude > 0)
				_commander.forward = new Vector3(aimVector.x, 0, aimVector.y);
		}

// attack test
		private void Attack()
		{
			weaponCollider.enabled = true;
			Debug.Log("Do Attack");
		}

		private void Skill01()
		{
			if(currentClass == PlayerClass.Warrior)
			{
				//1. 돌진하며 이동
				
				//2. 범위 내 적에게 피해
				
			}
			else
			{
				//TEST...
				Debug.Log("currentClass is not Warrior");
			}
			Debug.Log(currentClass + " : Skill01");
		}

		private void Skill02()
		{
			if(currentClass == PlayerClass.Warrior)
			{
				//3번째 공격마다 회복
				//Attack 함수에 조금 추가해주면 될듯
				//Button은 Disabled 되어야함.
			}
			else
			{
				//TEST...
				Debug.Log("currentClass is not Warrior");
			}
			Debug.Log(currentClass + " : Skill02");
		}

		private void Skill03()
		{
			if(currentClass == PlayerClass.Warrior)
			{
				//현재 체력의 40%를 소모해서 
				//공격력 방어력 상승
			}
			else
			{
				//TEST...
				Debug.Log("currentClass is not Warrior");
			}
			Debug.Log(currentClass + " : Skill03");
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(gameObject.transform.position,0.32f);	
>>>>>>> Seong_0.01
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
				_respawnInSeconds -= Runner.DeltaTime;

				if (_respawnInSeconds <= 0)
				{
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
<<<<<<< HEAD
=======
					
>>>>>>> Seong_0.01
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
<<<<<<< HEAD
			// TankTeleportOutEffect teleout = LocalObjectPool.Acquire(_teleportOutPrefab, transform.position, transform.rotation, null);
=======
			TankTeleportOutEffect teleout = LocalObjectPool.Acquire(_teleportOutPrefab, transform.position, transform.rotation, null);
>>>>>>> Seong_0.01
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
	}
}
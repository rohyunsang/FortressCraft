using Fusion;
using FusionHelpers;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace Agit.FortressCraft
{
	/// <summary>
	/// The Player class represent the players avatar - in this case the Tank.
	/// </summary>
	[RequireComponent(typeof(NetworkCharacterController))]
	public class RPG_Player : FusionPlayer
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
		private float MAX_HEALTH;

		[SerializeField] private Transform _commander;
		[SerializeField] private float _respawnTime;
		[SerializeField] private PlayerHPBar playerHPBar;

		[SerializeField] private AudioSource sound1;
		[SerializeField] private AudioSource sound2;
		[SerializeField] private AudioSource sound3;


		[Networked] public Stage stage { get; set; }
		[Networked] public float life { get; set; }  // player HP
		[Networked] private TickTimer respawnTimer { get; set; }
		[Networked] private TickTimer invulnerabilityTimer { get; set; }
		[Networked] public int lives { get; set; }
		[Networked] public bool ready { get; set; }
		[Networked] public float Defense { get; set; }
		[Networked] public float AttackDamage { get; set; }
        public int level { get; set; }


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
		private readonly static int animSkill1 = Animator.StringToHash("Base Layer.Skill1");
		//public Animator anim;
		private TickTimer attackInputTimer;
		private TickTimer skill1CoolTimer;
		private TickTimer skill2CoolTimer;

		private Vector2 lastDir = Vector2.left;
		private MagicianFire magicianFire;
		private ArrowVector arrowVector;
		private CommanderBodyCollider bodyCollider;
		[SerializeField] private WarriorWeaponCollider wairrorWeapon;
		[SerializeField] private WarriorChargeCollider warriorChargeCollider;
        private bool died = false;

		public JobType Job { get; set; }


		public string OwnType { get; set; }

		[Networked] public NetworkString<_32> PlayerName { get; set; }
		[SerializeField] TextMeshPro playerNameLabel;

		[Networked] public NetworkString<_256> LastPublicChat { get; set; }
		string currentChat = "";

		[Networked] public bool IsDestroyedAllCastle { get; set; }

		public bool isBuildCastle;

		[Networked] public int castleCount { get; set; }


		private void Awake()
		{
			_cc = GetComponent<NetworkCharacterController>();
			_collider = GetComponentInChildren<Collider>();
			_netAnimator = GetComponent<NetworkMecanimAnimator>();
			arrowVector = GetComponentInChildren<ArrowVector>();
			bodyCollider = GetComponentInChildren<CommanderBodyCollider>();
            level = 1;

			Job = JobType.Beginner;
			
		}

		public override void InitNetworkState()
		{
			stage = Stage.New;
			lives = MAX_LIVES;
			life = MAX_HEALTH;
			IsDestroyedAllCastle = false;
		}

		public override void Spawned()
		{
			base.Spawned();

            DontDestroyOnLoad(gameObject);

			changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

			// Proxies may not be in state "NEW" when they spawn, so make sure we handle the state properly, regardless of what it is
			OnStageChanged();

			_respawnInSeconds = 0;


			// PlayerName Change
			var fusionLauncher = FindObjectOfType<FusionLauncher>();

			if (fusionLauncher != null)
			{
				PlayerName = new NetworkString<_32>(fusionLauncher.playerName);
			}

			ChatSystem.instance.playerName = fusionLauncher.playerName;

			attackInputTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
			skill1CoolTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
			skill2CoolTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
        }

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			Debug.Log($"Despawned PlayerAvatar for PlayerRef {PlayerId}");
			base.Despawned(runner, hasState);
		}


		public override void FixedUpdateNetwork()
		{
			if (Object.HasStateAuthority)
			{
				CheckRespawn();

				if (isRespawningDone)
					ResetPlayer();
			}

			if (InputController.fetchInput)
			{
				if (GetInput(out NetworkInputData input))
				{
					Debug.Log(input.moveDirection);

                    MovePlayer(input.moveDirection.normalized, input.aimDirection.normalized);




					if (input.moveDirection.normalized != Vector2.zero)
					{
						lastDir = input.moveDirection.normalized;
					}
					if (input.moveDirection.normalized.x < -0.1f || input.moveDirection.normalized.y < -0.1f || input.moveDirection.normalized.x > 0.1f || input.moveDirection.normalized.y > 0.1f)
					{
						_netAnimator.Animator.SetBool("isMove", true);
					}
					else
					{
                        _netAnimator.Animator.SetBool("isMove", false);
					}
                    _oldInput = input;
				}
			}
		}
		public void Attack()  // Archer
		{
			
			_netAnimator.Animator.SetTrigger("Attack");
			attackInputTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
			
		}

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
					case nameof(IsDestroyedAllCastle):
                        break;
					case nameof(life):
						playerHPBar.SetHPBar(life);
						break;
                }
            }

            var interpolated = new NetworkBehaviourBufferInterpolator(this);
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
            ChatSystem.instance.chatDisplay.text += "\n" + "[" + PlayerName.ToString() + "]"+ " : " + LastPublicChat.ToString();
            ChatSystem.instance.chatInputField.text = "";
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
            lives = MAX_LIVES;
        }

        public void Respawn(float inSeconds = 0)
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
					break;
				case Stage.TeleportOut:
					break;
			}
			_collider.enabled = stage != Stage.Dead;
		}

		private void ResetPlayer()
		{
			Debug.Log($"Resetting player {PlayerId}, tick={Runner.Tick}, timer={respawnTimer.IsRunning}:{respawnTimer.TargetTick}, life={life}, lives={lives}, hasStateAuth={Object.HasStateAuthority} to state={stage}");
			stage = Stage.Active;
		}

		public void CheckDamaged()
        {
			if (died) return;
			if( bodyCollider.Damaged > 0.0f )
            {
				life -= bodyCollider.Damaged * (1.0f - 0.01f * Defense);

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

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
    }
}
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
		private float MAX_HEALTH;

		//public float HP { get; set; }
		public SpawnCastle _spawnCastle;
		[SerializeField] private Transform _commander;
		[SerializeField] private TankTeleportInEffect _teleportIn;
		[SerializeField] private TankTeleportOutEffect _teleportOutPrefab;
		[SerializeField] private float _respawnTime;
		[SerializeField] private PlayerHPBar playerHPBar;


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
		private ArcherFire archerFire;
		private MagicianFire magicianFire;
		private ArrowVector arrowVector;
		private CommanderBodyCollider bodyCollider;
		[SerializeField] private WarriorWeaponCollider wairrorWeapon;
		[SerializeField] private WarriorChargeCollider warriorChargeCollider;
        private bool died = false;

		public JobType Job { get; set; }

		private Button attackBtn;
		private Button skill1Btn;
		private Button skill2Btn;
		private Image[] skill1BtnImages;
		private Image[] skill2BtnImages;

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
			archerFire = GetComponentInChildren<ArcherFire>();
			magicianFire = GetComponentInChildren<MagicianFire>();
			arrowVector = GetComponentInChildren<ArrowVector>();
			bodyCollider = GetComponentInChildren<CommanderBodyCollider>();
            level = 1;

			if (archerFire != null) Job = JobType.Archer;
			else if (magicianFire != null) Job = JobType.Magician;
			else
			{
				Job = JobType.Warrior;
			}
			
            SetMaxHPByLevel(level, Job);
		}

		public void SetMaxHPByLevel(int level, JobType jobType)
		{
			MAX_HEALTH = GoogleSheetManager.GetCommanderData(level, jobType).HP;
			Debug.Log("Max HP: " + MAX_HEALTH);
		}

		public void SetAttack(int level, JobType jobType)
		{
            AttackDamage = GoogleSheetManager.GetCommanderData(level,jobType).Attack;
        }

		public void SetDefenseHPByLevel(int level, JobType jobType)
		{
			Defense = GoogleSheetManager.GetCommanderData(level, jobType).Defense;
		}

		public float GetNeedExpByLevel(int level, JobType jobType)
		{
			return GoogleSheetManager.GetCommanderData(level, jobType).NeedExp;
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

			ready = false;

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

			StartCoroutine(AutoHeal());
			castleCount = 1;

        }

		public void UpdateBattleSetting()
		{
			if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Battle")) return;

			GameObject bunttonObject = GameObject.Find("AttackBtnGroups");

			if (bunttonObject == null) return;

			attackBtn = bunttonObject.GetComponentInChildren<Button>();
			attackBtn.onClick.AddListener(Attack);

			skill1Btn = GameObject.Find("SkillBtnGroups_001").GetComponentInChildren<Button>();
			skill1Btn.onClick.AddListener(Skill1);
			skill1BtnImages = skill1Btn.GetComponentsInChildren<Image>();

			skill2Btn = GameObject.Find("SkillBtnGroups_002").GetComponentInChildren<Button>();
			skill2Btn.onClick.AddListener(Skill2);
			skill2BtnImages = skill2Btn.GetComponentsInChildren<Image>();

			if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
			{
				switch (gameManager.TryGetPlayerId(Runner.LocalPlayer))
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

				RPCSetType("Unit_" + OwnType);

				if (Job == JobType.Archer)
				{
					archerFire.OwnType = OwnType;
				}
				else if (Job == JobType.Magician)
				{
					magicianFire.OwnType = OwnType;
				}
				else if (Job == JobType.Warrior)
                {
                    wairrorWeapon.OwnType = OwnType;
					warriorChargeCollider.OwnType = OwnType;
                }
			}
		}

		private IEnumerator AutoHeal()
		{
			while (true)
			{
				if (!died)
				{
					CommanderData commanderData = GoogleSheetManager.GetCommanderData(level, Job);
					life *= (1.0f + 0.01f * commanderData.HealPerSecond);
					if (life > commanderData.HP)
					{
						life = commanderData.HP;
					}
				}

				yield return new WaitForSeconds(5.0f);
			}
		}


		[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
		public void RPC_SetDark(PlayerRef playerRef)
		{
			if (PlayerId == playerRef && HasStateAuthority)
			{
				if (PlayerId == playerRef && HasStateAuthority)
				{
					GameObject.Find("UIManager").GetComponent<UIManager>().OnDarkFilter();
				}
			}
		}

		[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
		public void RPC_SetBright(PlayerRef playerRef)
		{
			if (PlayerId == playerRef && HasStateAuthority)
			{
				GameObject.Find("UIManager").GetComponent<UIManager>().OffDarkFilter();
			}
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCSetType(string tag)
		{
			transform.Find("UnitRoot").gameObject.tag = tag;
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCSetLevel(int level)
		{
			this.level = level;
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			Debug.Log($"Despawned PlayerAvatar for PlayerRef {PlayerId}");
			base.Despawned(runner, hasState);
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCSetScale(Vector3 scale)
		{
			transform.localScale = scale;
		}

		public void ExpCheck()
		{
			if (RewardManager.Instance != null)
			{
				float needExp = GetNeedExpByLevel(level, Job);
				if (RewardManager.Instance.Exp >= needExp)
				{
					RewardManager.Instance.Exp -= needExp;
					++level;
					RPCSetLevel(level);
					UpdateProperty();
				}
			}
		}

		public void UpdateProperty()
		{
			// HP
			SetMaxHPByLevel(level, Job);
			life = MAX_HEALTH;

			// Defense
			SetDefenseHPByLevel(level, Job);
		}

		public override void FixedUpdateNetwork()
		{
			if (attackBtn == null) UpdateBattleSetting();

			if (Object.HasStateAuthority)
			{
				CheckRespawn();

				if (isRespawningDone)
					ResetPlayer();
			}

			if (died) return;
			animState = _netAnimator.Animator.GetCurrentAnimatorStateInfo(0);

			UpdateBtnColor();

			ExpCheck();

			if (InputController.fetchInput)
			{
				if (GetInput(out NetworkInputData input))
				{
					if (lastDir.x > 0.0f)
					{
						RPCSetScale(new Vector3(-1 * Mathf.Abs(transform.localScale.x),
									transform.localScale.y, transform.localScale.z));
					}
					else
					{
						RPCSetScale(new Vector3(Mathf.Abs(transform.localScale.x),
									transform.localScale.y, transform.localScale.z));
					}

					if (animState.fullPathHash != animAttack &&
						animState.fullPathHash != animSkill1)
					{
						MovePlayer(input.moveDirection.normalized, input.aimDirection.normalized);
					}

					if (input.moveDirection.normalized != Vector2.zero)
					{
						lastDir = input.moveDirection.normalized;
					}

					if (arrowVector != null)
					{
						arrowVector.TargetDirection = lastDir;
					}

					if (input.moveDirection.normalized != Vector2.zero)
					{
						_netAnimator.Animator.SetBool("isMove", true);
					}
					else
					{
						_netAnimator.Animator.SetBool("isMove", false);
					}

					if (isBuildCastle && Object.HasStateAuthority && input.WasPressed(NetworkInputData.BUTTON_TOGGLE_SPAWNCASTLE, _oldInput))
					{
						RPC_CastleCount();
                        _spawnCastle.SpawnCastleObject();
                    }

                    _oldInput = input;
				}
			}
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_CastleCount()
        {
            castleCount++;
        }

        private void UpdateBtnColor()
		{
			if (skill1CoolTimer.Expired(Runner) && skill1Btn != null)
			{
				foreach (Image btnImage in skill1BtnImages)
				{
					btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 1.0f);
				}
			}
			else if (skill1Btn != null)
			{
				foreach (Image btnImage in skill1BtnImages)
				{
					btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 0.6f);
				}
			}

			if (skill2CoolTimer.Expired(Runner) && skill2Btn != null)
			{
				foreach (Image btnImage in skill2BtnImages)
				{
					btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 1.0f);
				}
			}
			else if (skill2Btn != null)
			{
				foreach (Image btnImage in skill2BtnImages)
				{
					btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 0.6f);
				}
			}
		}

		public void Attack()  // Archer
		{
			if (attackInputTimer.Expired(Runner))
			{
				if (Job == JobType.Archer)
				{
					archerFire.FireDirection = lastDir;
					archerFire.SetDamageByLevel(level, Job);
				}
				else if (Job == JobType.Magician)
				{
					magicianFire.SetDamageByLevel(level, Job);
				}
				else if( Job == JobType.Warrior)
				{
					SetAttack(level, Job);
					wairrorWeapon.Damage = AttackDamage;
				}
				_netAnimator.Animator.SetTrigger("Attack");
				attackInputTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
			}
		}

		public void Skill1()
		{
			if (skill1CoolTimer.Expired(Runner))
			{
				if (Job == JobType.Archer)
				{
					archerFire.FireDirection = lastDir;
					archerFire.SetDamageByLevel(level, Job);
					skill1CoolTimer = TickTimer.CreateFromSeconds(Runner, 5.0f);
                    _netAnimator.Animator.SetTrigger("Skill1");
                }
				else if (Job == JobType.Magician)
				{
					RPCMagicSetting();
					skill1CoolTimer = TickTimer.CreateFromSeconds(Runner, 5.0f);
                    _netAnimator.Animator.SetTrigger("Skill1");
                }
                else if (Job == JobType.Warrior)
				{
					RPC_ChargeStartCallback();
					warriorChargeCollider.GetComponent<WarriorChargeCollider>().Damage = AttackDamage;

					_netAnimator.Animator.SetTrigger("Skill1");
					_cc.isCharge = true;
					skill1CoolTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
                    Invoke("ChargeFinishCallback", 0.15f);
                }
			}
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPCMagicSetting()
        {
			magicianFire.FireDirection = lastDir;
			magicianFire.SetDamageByLevel(level, Job);
		}
		private void ChargeFinishCallback()
		{
			_cc.isCharge = false;
			RPC_ChargeEndCallback();
        }


        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_ChargeStartCallback()
        {
            wairrorWeapon.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("WarriorCharging");
            warriorChargeCollider.gameObject.SetActive(true);
        }

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_ChargeEndCallback()
        {
            wairrorWeapon.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            warriorChargeCollider.gameObject.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        public void Skill2() // Archer
        {
            if (skill2CoolTimer.Expired(Runner))
			{
				if( Job == JobType.Archer )
                {
					archerFire.SetDamageByLevel(level, Job);
					skill2CoolTimer = TickTimer.CreateFromSeconds(Runner, 5.0f);
				}
				else if( Job == JobType.Magician )
                {
					magicianFire.SetDamageByLevel(level, Job);
					skill2CoolTimer = TickTimer.CreateFromSeconds(Runner, 10.0f);
                }
				else if(Job == JobType.Warrior)
				{
                    // Healing 부분 
					skill2CoolTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
                    float currentMaxHp = GoogleSheetManager.GetCommanderData(level, Job).HP;
					
					if(currentMaxHp < life + currentMaxHp * 0.3f)
					{
						life = currentMaxHp;
					}
					else
					{
						life += currentMaxHp * 0.3f;
					}
                }

				_netAnimator.Animator.SetTrigger("Skill2");
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

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_CastleCountDown(PlayerRef playerRef)
        {
            if (PlayerId == playerRef && HasStateAuthority)
            {
				RPC_CastleCountDownInternal();
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_CastleCountDownInternal()
        {
            castleCount--;
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_DestroyedAllCastle(PlayerRef playerRef)
		{
			if (PlayerId == playerRef && HasStateAuthority)
			{
                IsDestroyedAllCastle = true;
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
			//_cc.Move(new Vector3(aimVector.x, aimVector.y, 0));

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
					RPC_SetBright(PlayerId);
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
			RPC_SetDark(PlayerId);
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
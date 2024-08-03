using UnityEngine;
using Fusion;
using FusionHelpers;
//using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

namespace Agit.FortressCraft
{
    public class NormalUnitRigidBodyMovement : NetworkBehaviour
    {
        public NormalUnitSpawner Spawner { get; set; }
        private Transform[] grounds = new Transform[4];
        private Transform middlePoint = null;
        [SerializeField] private int testSpeed;

        public string TargetString { get; set; }

        public bool AttackEnabled { get; set; }
        public string TargetGround { get; set; }
        public string TargetUnit { get; set; }
        public float Damage { get; set; }
        public float Defense { get; set; }  // 0~1 사이 값으로 사용, 받은 대미지에 곱해서 적용
        public string OwnType { get; set; }
        public bool NoReward { get; set; }
        public int gold = 10;
        public int exp = 10;

        private readonly static int animAttackBow =
            Animator.StringToHash("Base Layer.AttackState");
        private readonly static int animDie =
            Animator.StringToHash("Base Layer.Death");
        private AnimatorStateInfo animatorState;

        private bool initialized = false;
        private string nowGround;

        private Animator animator;
        public NetworkMecanimAnimator _netAnimator;
        private BodyCollider bodyCollider;
        private NormalUnitFire normalUnitFire;

        private NetworkRigidbody2D _rb;
        public float HP { get; set; }

        private TickTimer dieTimer;

        void Awake()
        {
            initialized = false;
            _rb = GetComponent<NetworkRigidbody2D>();

            _netAnimator = GetComponent<NetworkMecanimAnimator>();
            animator = GetComponent<Animator>();
            bodyCollider = GetComponentInChildren<BodyCollider>();
            normalUnitFire = GetComponentInChildren<NormalUnitFire>();

            // test
            AttackEnabled = true;
            HP = 100;
            grounds[0] = GameObject.Find("SpawnPoint 1").transform;
            grounds[1] = GameObject.Find("SpawnPoint 2").transform;
            grounds[2] = GameObject.Find("SpawnPoint 3").transform;
            grounds[3] = GameObject.Find("SpawnPoint 4").transform;
        }

        private float GetDistanceXYSquared(Transform t)
        {
            float result = 0.0f;

            result = Mathf.Pow((t.position.x - transform.position.x), 2) +
                     Mathf.Pow((t.position.y - transform.position.y), 2);

            return result;
        }

        public void Initializing()
        {
            if (initialized)
            {
                if (TargetString != Spawner.Target) // 타겟 변경됐는데 다리 위
                {
                    if (nowGround == "Bridge")
                    {
                        float minDist = GetDistanceXYSquared(grounds[0]);
                        int idx = 0;
                        for (int i = 1; i < 4; ++i)
                        {
                            float tempDist = GetDistanceXYSquared(grounds[i]);

                            if (tempDist < minDist)
                            {
                                minDist = tempDist;
                                idx = i;
                            }
                        }
                        middlePoint = grounds[idx];
                    }
                    else if (nowGround == "CrossBridge")
                    {
                        middlePoint = Spawner.Center;
                    }
                }

                // RPC Properties
                TargetString = Spawner.Target;
                AttackEnabled = Spawner.AttackEnabled;
                Damage = Spawner.Damage;
                Defense = Spawner.Defense;
            }

            TargetGround = "Ground_" + TargetString;
            TargetUnit = "Unit_" + TargetString;
            initialized = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (!initialized) return;
            
            if (!Attack()) MoveToTarget();
            else _rb.Rigidbody.velocity = Vector2.zero;

            animatorState = animator.GetCurrentAnimatorStateInfo(0);

            if (animatorState.fullPathHash != animAttackBow)
            {
                if (_rb.Rigidbody.velocity.x != 0.0f || _rb.Rigidbody.velocity.y != 0.0f)
                {
                    _netAnimator.Animator.SetTrigger("Run");
                }
                else
                {
                    _netAnimator.Animator.SetTrigger("Idle");
                }
            }
            else
            {
                if (_rb.Rigidbody.velocity.x != 0.0f || _rb.Rigidbody.velocity.y != 0.0f)
                {
                    _netAnimator.Animator.SetTrigger("Run");
                }
            }

            CheckDamaged();

            if (dieTimer.Expired(Runner))
            {
                --Spawner.NowUnitCount;
                dieTimer = TickTimer.None;
                NetworkObjectReleaseContext context = new NetworkObjectReleaseContext(Object, Spawner.id, false, false);
                Spawner.poolManager.ReleaseInstance(Runner, context);
            }

            Initializing();
            Debug.Log("Unit Attack: " + Damage);
        }



        protected bool AttackAllTarget()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3.0f);

            // 적 탐색 
            foreach (Collider2D col in cols)
            {
                if (col.tag.StartsWith("Unit"))
                {
                    if (col.CompareTag("Unit_" + OwnType)) continue;
                    //Debug.Log(col.tag + " " + OwnType);
                    normalUnitFire.TargetTranform = col.transform;
                    normalUnitFire.SecondTargetUnit = col.tag;

                    if (col.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-1.0f * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        transform.localScale = new Vector3(1.0f * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y, transform.localScale.z);
                    }

                    if (animatorState.fullPathHash != animAttackBow)
                    {
                        _netAnimator.Animator.SetTrigger("Attack");
                    }

                    return true;
                }
            }

            return false;
        }

        protected bool Attack()
        {
            if (!AttackEnabled)
            {
                return false;
            }

            if (Spawner.Target.CompareTo(OwnType) == 0)
            {
                return AttackAllTarget();
            }

            // Debug.Log("Setting: " + Spawner.Target + " " + OwnType);

            Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3.0f);

            // 적 탐색 
            foreach (Collider2D col in cols)
            {
                if (col.CompareTag(TargetUnit) && TargetUnit != "Unit_"+ OwnType )
                {
                    // Debug.Log(TargetUnit + " " + OwnType);
                    normalUnitFire.TargetTranform = col.transform;

                    if (col.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-1.0f * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        transform.localScale = new Vector3(1.0f * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y, transform.localScale.z);
                    }

                    if (animatorState.fullPathHash != animAttackBow)
                    {
                        _netAnimator.Animator.SetTrigger("Attack");
                    }

                    return true;
                }
            }

            return false;
        }

        public void FireArrow()
        {
            normalUnitFire.Fire(Runner);
        }

        private void Die()
        {
            //Debug.Log(HP);
            if (HP <= 0.0f)
            {
                _netAnimator.Animator.SetTrigger("Die");
                dieTimer = TickTimer.CreateFromSeconds(Runner, 0.26f);
            }
        }

        private void MoveToTarget()
        {
            Transform targetGround;

            if (middlePoint == null)
            {
                if (TargetGround.CompareTo("Ground_A") == 0)
                {
                    targetGround = grounds[0];
                }
                else if (TargetGround.CompareTo("Ground_B") == 0)
                {
                    targetGround = grounds[1];
                }
                else if (TargetGround.CompareTo("Ground_C") == 0)
                {
                    targetGround = grounds[2];
                }
                else
                {
                    targetGround = grounds[3];
                }

                if (TargetGround.CompareTo(nowGround) == 0)
                {
                    _rb.Rigidbody.velocity = Vector2.zero;
                    return;
                }
            }
            else
            {
                targetGround = middlePoint;
            }

            if (Mathf.Sqrt(GetDistanceXYSquared(targetGround)) < 0.3f)
            {
                if (middlePoint == null)
                {
                    _rb.Rigidbody.velocity = Vector2.zero;
                    return;
                }
                else
                {
                    middlePoint = null;
                    return;
                }
            }

            Vector3 movDir = targetGround.position - transform.position;
            Vector3 movDirNormalized = movDir.normalized;

            _rb.Rigidbody.velocity = movDirNormalized * testSpeed;

            if (movDir.x > 0)
            {
                transform.localScale = new Vector3(-1.0f * Mathf.Abs(transform.localScale.x),
                                            transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(1.0f * Mathf.Abs(transform.localScale.x),
                                            transform.localScale.y, transform.localScale.z);
            }
        }

        private void CheckDamaged()
        {
            if (bodyCollider.Damaged > 0.0f)
            {
                HP -= Defense * bodyCollider.Damaged;
                RPCSyncHP(HP);
                bodyCollider.Damaged = 0.0f;
                _netAnimator.Animator.SetTrigger("Damaged");
                //Debug.Log("HP: " + HP);
                Die();
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSyncHP(float HP)
        {
            this.HP = HP;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground_A"))
            {
                nowGround = "Ground_A";
            }
            else if (collision.CompareTag("Ground_B"))
            {
                nowGround = "Ground_B";
            }
            else if (collision.CompareTag("Ground_C"))
            {
                nowGround = "Ground_C";
            }
            else if (collision.CompareTag("Ground_D"))
            {
                nowGround = "Ground_D";
            }
            else if (collision.CompareTag("Bridge"))
            {
                nowGround = "Bridge";
            }
            else if (collision.CompareTag("CrossBridge"))
            {
                nowGround = "CrossBridge";
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetActive()
        {
            gameObject.SetActive(true);
            Debug.Log("Unit Activatied? : " + gameObject.activeSelf);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetUnactive()
        {
            gameObject.SetActive(false);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetPos(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}
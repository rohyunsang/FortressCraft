using UnityEngine;
using Fusion;
using FusionHelpers;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

namespace Agit.FortressCraft
{
    public class NormalUnitRigidBodyMovement : NetworkBehaviour
    {
        public NormalUintSpawner Spawner { get; set; }
        private Transform ground_A;
        private Transform ground_B;
        private Transform ground_C;
        private Transform ground_D;
        [SerializeField] private int testSpeed;

        public string TargetString { get; set; }

        public bool AttackEnabled { get; set; }
        public string TargetGround { get; set; }
        public string TargetUnit { get; set; }
        public float Damage { get; set; }
        public float Defense { get; set; }  // 0~1 사이 값으로 사용, 받은 대미지에 곱해서 적용

        private readonly static int animAttackBow =
            Animator.StringToHash("Base Layer.2_Attack_Bow");
        private AnimatorStateInfo animatorState;

        private bool initialized = false;
        private string nowGround;

        private Animator animator;
        private NetworkMecanimAnimator _netAnimator;
        private BodyCollider bodyCollider;
        private NormalUnitFire normalUnitFire;

        private NetworkRigidbody2D _rb;
        public float HP { get; set; }

        private TickTimer dieTimer;

        public override void Spawned()
        {
            Debug.Log("Unit Spawned");
        }

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

            ground_A = GameObject.Find("Castle1").transform;
            ground_B = GameObject.Find("Castle2").transform;
            ground_C = GameObject.Find("Castle3").transform;
            ground_D = GameObject.Find("Castle4").transform;
        }

        public void Initializing()
        {
            if (initialized)
            {   // RPC Properties
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

            if (animatorState.fullPathHash != animAttackBow )
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

            CheckDamaged();

            if (dieTimer.Expired(Runner))
            {
                dieTimer = TickTimer.None;
                NetworkObjectReleaseContext context = new NetworkObjectReleaseContext(Object, Spawner.id, false, false);
                Spawner.poolManager.ReleaseInstance(Runner, context);
            }

            Initializing();
        }

        protected bool Attack()
        {
            if (!AttackEnabled)
            {
                return false;
            }

            Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3.0f);

            // 적 탐색 
            foreach (Collider2D col in cols)
            {
                if (col.CompareTag(TargetUnit))
                {
                    //Debug.Log("Target in");
                    normalUnitFire.TargetTranform = col.transform;

                    if (col.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);
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

            if (TargetGround.CompareTo("Ground_A") == 0)
            {
                targetGround = ground_A;
            }
            else if (TargetGround.CompareTo("Ground_B") == 0)
            {
                targetGround = ground_B;
            }
            else if (TargetGround.CompareTo("Ground_C") == 0)
            {
                targetGround = ground_C;
            }
            else
            {
                targetGround = ground_D;
            }

            //Debug.Log(targetGround);
            //Debug.Log(TargetGround + " " + nowGround);
            if (TargetGround.CompareTo(nowGround) == 0)
            {
                //Debug.Log("Stoped");
                _rb.Rigidbody.velocity = Vector2.zero;
                //_netAnimator.Animator.SetTrigger("Idle");
                return;
            }

            Vector3 movDir = targetGround.position - transform.position;
            Vector3 movDirNormalized = movDir.normalized;

            _rb.Rigidbody.velocity = movDirNormalized * testSpeed;

            if (movDir.x > 0)
            {
                transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);
            }
        }

        private void CheckDamaged()
        {
            if (bodyCollider.Damaged > 0.0f)
            {
                HP -= Defense * bodyCollider.Damaged;
                bodyCollider.Damaged = 0.0f;
                _netAnimator.Animator.SetTrigger("Damaged");
                Die();
                //Debug.Log(HP);
            }
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
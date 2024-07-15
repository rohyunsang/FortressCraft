using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;
using static UnityEngine.EventSystems.PointerEventData;

public class NormalUnitRigidBodyMovement : NetworkBehaviour
{
    private Transform ground_A;
    private Transform ground_B;
    private Transform ground_C;
    private Transform ground_D;
    [SerializeField] private int testSpeed;
    public string TargetString { get; set; }

    public bool AttackEnabled { get; set; }
    public string TargetGround { get; set; }
    public string TargetUnit { get; set; }

    private bool initialized = false;
    private string nowGround;

    private Animator animator;
    private NetworkMecanimAnimator _netAnimator;
    private BodyCollider bodyCollider;
    private NormalUnitFire normalUnitFire;

    private NetworkRigidbody2D _rb;
    private float HP;

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

        ground_A = GameObject.Find("Castle1").transform;
        ground_B = GameObject.Find("Castle2").transform;
        ground_C = GameObject.Find("Castle3").transform;
        ground_D = GameObject.Find("Castle4").transform;
    }

    public void Initializing()
    {
        TargetGround = "Ground_" + TargetString;
        TargetUnit = "Unit_" + TargetString;
        initialized = true;
    }

    public override void Spawned()
    {
        Runner.SetPlayerAlwaysInterested(Object.InputAuthority, Object, true);
    }


    public override void FixedUpdateNetwork()
    {
        if (!initialized) return;
        //Debug.Log("NormalUnitRigidBodyMovement is working");
        if (!Attack()) MoveToTarget();
        else _rb.Rigidbody.velocity = Vector2.zero;
        
        CheckDamaged();
        
        
        if( dieTimer.Expired(Runner) )
        {
            Destroy(this.gameObject);
        }
    }

    protected bool Attack()
    {
        if (!AttackEnabled)
        {
            _netAnimator.Animator.SetTrigger("Idle");
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

                if( col.transform.position.x > transform.position.x )
                {
                    transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);
                }

                _netAnimator.Animator.SetTrigger("Attack");
                
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
            _netAnimator.Animator.SetTrigger("Idle");
            return;
        }


        Vector3 movDir = targetGround.position - transform.position;
        Vector3 movDirNormalized = movDir.normalized;
        //Debug.Log( targetGround + " " + movDir);
        if (Mathf.Abs(movDir.x) > 0.1 && Mathf.Abs(movDir.y) > 0.1f)
        {
            _netAnimator.Animator.SetTrigger("Run");
        }

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
            HP -= bodyCollider.Damaged;
            bodyCollider.Damaged = 0.0f;
            _netAnimator.Animator.SetTrigger("Damaged");
            Die();
            Debug.Log(HP);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
        else
        {
            nowGround = "Bridge";
        }
        //Debug.Log(nowGround);
    }
}
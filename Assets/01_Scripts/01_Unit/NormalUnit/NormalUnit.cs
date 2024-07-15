using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnit : Unit
{
    private Transform ground_A;
    private Transform ground_B;
    private Transform ground_C;
    private Transform ground_D;
    [SerializeField] private int testSpeed;
    [SerializeField] private string testTargetString = "";

    public bool AttackEnabled { get; set; }
    public string TargetGround { get; set; }
    public string TargetUnit { get; set; }

    private string nowGround;

    private Animator animator;
    private Rigidbody2D rb;
    private BodyCollider bodyCollider;
    private NormalUnitFire normalUnitFire;

    private void Awake()
    {
        if (testTargetString != "")
        {
            TargetGround = "Ground_" + testTargetString;
            TargetUnit = "Unit_" + testTargetString;
        }
        animator = GetComponent<Animator>();
        bodyCollider = GetComponentInChildren<BodyCollider>();
        normalUnitFire = GetComponentInChildren<NormalUnitFire>();
        rb = GetComponent<Rigidbody2D>();

        // test
        AttackEnabled = true;
        HP = 100;

        ground_A = GameObject.Find("Map1").transform;
        ground_B = GameObject.Find("Map2").transform;
        ground_C = GameObject.Find("Map3").transform;
        ground_D = GameObject.Find("Map4").transform;
    }

    private void FixedUpdate()
    {
        if (!Attack()) MoveToTarget();
        else rb.velocity = Vector2.zero;

        CheckDamaged();
    }

    protected override bool Attack()
    {
        if (!AttackEnabled)
        {
            animator.SetTrigger("Idle");
            return false;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3.0f);

        // 적 탐색 
        foreach (Collider2D col in cols)
        {
            if (col.CompareTag(TargetUnit))
            {
                normalUnitFire.TargetTranform = col.transform;
                animator.SetTrigger("Attack");
                return true;
            }
        }


        return false;
    }

    protected override void Die()
    {
        //Debug.Log(HP);
        if (HP <= 0.0f)
        {
            animator.SetTrigger("Die");
            Invoke("DestroySelf", 0.26f);
        }
    }

    protected override void MoveToTarget()
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

        //Debug.Log(TargetGround + " " + nowGround);
        if (TargetGround.CompareTo(nowGround) == 0)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Idle");
            return;
        }


        Vector3 movDir = targetGround.position - transform.position;
        Vector3 movDirNormalized = movDir.normalized;
        //Debug.Log(movDir);
        if (Mathf.Abs(movDir.x) > 0.5 && Mathf.Abs(movDir.y) > 0.5f)
        {
            animator.SetTrigger("Run");
        }

        rb.velocity = movDirNormalized * testSpeed;

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
            animator.SetTrigger("Damaged");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;



namespace Agit.FortressCraft
{
    public enum Monster_NormalState
    {
        IDLE,
        RUN,
        ATTACK,
        NON
    }

    public class Monster_NormalController : MonsterController
    {
        private Monster_NormalState state = Monster_NormalState.NON;
        private MonsterAttackCollider attackCollider;
        private delegate void Attack();
        Attack AttackFunc;
        [SerializeField] NetworkObject obj;

        public override void Spawned()
        {
            base.Spawned();
            Hp = hpMax;
            attackCollider = GetComponentInChildren<MonsterAttackCollider>();

            if( monsterData.Type == MonsterType.NORMAL )
            {
                AttackFunc = ActionAttack;
            }
            else if( monsterData.Type == MonsterType.BOW )
            {
                AttackFunc = ActionBow;
            }
        }

        // FixedUpdateNetwork는 Athority 있는 거에서만 돌아서 FixedUpdate에서 처리 필요 
        private void FixedUpdate()
        {
            if (Runner == null) return;

            if (Runner.IsSharedModeMasterClient)
            {
                rb.velocity = Vector2.zero;
                MonsterAI();
            }
        }

        public void SetState(Monster_NormalState state, float nextDelay)
        {
            if (!timeCheck()) return;

            acted = false;
            startTime = Time.fixedTime;
            this.state = state;
            this.nextDelay = nextDelay;
        }

        public override void MonsterAI()
        {
            if (Hp <= 0.0f) return;

            switch (state)
            {
                case Monster_NormalState.IDLE:
                    ActionIdle();
                    break;
                case Monster_NormalState.RUN:
                    ActionRun();
                    break;
                case Monster_NormalState.ATTACK:
                    AttackFunc();
                    break;

            }
        }

        private void ActionIdle()
        {
            if (acted) return;
            animator.SetTrigger("Idle");
            acted = true;
        }

        private void ActionRun()
        {
            if (!acted)
            {
                dir = Vector2.zero;
                Transform Target = null;
                Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 4.0f);

                // 적 탐색 
                foreach (Collider2D col in cols)
                {
                    if (col.tag.StartsWith("Unit") && !col.CompareTag("Unit_Monster"))
                    {
                        Target = col.transform;
                        break;
                    }
                }

                if (Target != null)
                {
                    dir = (Target.position - transform.position).normalized;
                }
                else
                {
                    // 타겟이 없을 시 랜덤 단위 벡터 
                    float angle = Random.Range(0f, Mathf.PI * 2);
                    dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                
                animator.SetTrigger("Run");
                acted = true;
            }

            if (dir.x > 0.001f || dir.y > 0.001f)
            {
                rb.velocity = dir * movingWeight;
            }

            if (rb.velocity.x > 0)
            {
                rb.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1.0f,
                                                    transform.localScale.y, transform.localScale.z);
            }
            else
            {
                rb.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
                                                    transform.localScale.y, transform.localScale.z);
            }
        }

        private void ActionAttack()
        {
            if (acted) return;

            attackCollider.Damage = damage * 2.0f;
            animator.SetTrigger("AttackNormal");
            acted = true;
        }

        private void ActionBow()
        {
            if (acted) return;

            attackCollider.Damage = damage * 3.0f;
            animator.SetTrigger("AttackBow");
            acted = true;
        }

        public void Fire()
        {
            Transform Target = null;
            Collider2D[] cols = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 4.0f);

            // 적 탐색 
            foreach (Collider2D col in cols)
            {
                if (col.tag.StartsWith("Unit") && !col.CompareTag("Unit_Monster"))
                {
                    Target = col.transform;
                    break;
                }
            }

            if (Target == null) return;

            NetworkObject fireObj = Runner.Spawn(obj, transform.position, Quaternion.identity);
            MonsterArrow monsterArrow = fireObj.GetComponent<MonsterArrow>();
            monsterArrow.TargetTransform = Target;

            MonsterAttackCollider monsterAttackCollider = fireObj.GetComponent<MonsterAttackCollider>();
            monsterAttackCollider.Damage = damage;
        }
    }
}


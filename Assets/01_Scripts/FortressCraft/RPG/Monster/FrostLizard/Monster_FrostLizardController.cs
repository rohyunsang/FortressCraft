using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;



namespace Agit.FortressCraft
{
    public enum Monster_FrostLizardState
    {
        IDLE,
        WALK,
        BREATH,
        TAILATTACK,
        SLAM,
        NON
    }

    public class Monster_FrostLizardController : MonsterController
    {
        private Monster_FrostLizardState state = Monster_FrostLizardState.NON;
        private Monster_FrostLizardAttackCollider attackCollider;
        private Vector2 dir;

        public override void Spawned()
        {
            base.Spawned();
            Hp = hpMax;
            attackCollider = GetComponentInChildren<Monster_FrostLizardAttackCollider>();
        }

        public override void FixedUpdateNetwork()
        {
            rb.velocity = Vector2.zero;
            MonsterAI();
        }

        public void SetState(Monster_FrostLizardState state, float nextDelay)
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
                case Monster_FrostLizardState.IDLE:
                    ActionIdle();
                    break;
                case Monster_FrostLizardState.WALK:
                    ActionWalk();
                    break;
                case Monster_FrostLizardState.BREATH:
                    ActionBreath();
                    break;
                case Monster_FrostLizardState.TAILATTACK:
                    ActionTailAttack();
                    break;
                case Monster_FrostLizardState.SLAM:
                    ActionSlam();
                    break;

            }
        }

        private void ActionIdle()
        {
            if (acted) return;
            animator.SetTrigger("Idle");
            acted = true;
        }

        private void ActionWalk()
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

                if( Target != null )
                {
                    dir = (Target.position - transform.position).normalized;
                }
                else
                {
                    // 타겟이 없을 시 랜덤 단위 벡터 
                    float angle = Random.Range(0f, Mathf.PI * 2);
                    dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }

                animator.SetTrigger("Walk");
                acted = true;
            }

            if( dir.x > 0.001f || dir.y > 0.001f )
            {
                rb.velocity = dir * movingWeight;
            }

            if( rb.velocity.x > 0 )
            {
                rb.transform.localScale = new Vector3( Mathf.Abs( transform.localScale.x) * -1.0f,
                                                    transform.localScale.y, transform.localScale.z);
            }
            else
            {
                rb.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
                                                    transform.localScale.y, transform.localScale.z);
            }
        }

        private void ActionBreath()
        {
            if (acted) return;

            attackCollider.Damage = 300.0f;
            animator.SetTrigger("Breath");
            acted = true;
        }

        private void ActionTailAttack()
        {
            if (acted) return;

            attackCollider.Damage = 100.0f;
            animator.SetTrigger("TailAttack");
            acted = true;
        }

        private void ActionSlam()
        {
            if (acted) return;

            attackCollider.Damage = 500.0f;
            animator.SetTrigger("Slam");
            acted = true;
        }
    }
}


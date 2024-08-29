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
        private Vector2 dir;

        public override void Spawned()
        {
            base.Spawned();
            Hp = hpMax;
            attackCollider = GetComponentInChildren<MonsterAttackCollider>();
        }

        // FixedUpdateNetwork는 Athority 있는 거에서만 돌아서 FixedUpdate에서 처리 필요 
        private void FixedUpdate()
        {
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
                    ActionAttack();
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
            
        }

        private void ActionAttack()
        {
            if (acted) return;

            attackCollider.Damage = damage * 3.0f;
            animator.SetTrigger("Attack");
            acted = true;
        }

    }
}


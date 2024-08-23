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
        NON
    }

    public class Monster_FrostLizardController : MonsterController
    {
        private Monster_FrostLizardState state = Monster_FrostLizardState.NON;
        private Monster_FrostLizardAttackCollider attackCollider;

        public override void Spawned()
        {
            base.Spawned();
            Hp = hpMax;
            attackCollider = GetComponentInChildren<Monster_FrostLizardAttackCollider>();
        }

        public override void FixedUpdateNetwork()
        {
            // 속도 설정 같은 거 때문에 여기에 있어야 함 
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
                animator.SetTrigger("Walk");
                acted = true;
            }
        }

        private void ActionBreath()
        {
            if (acted) return;
            attackCollider.Damage = 500.0f;
            animator.SetTrigger("Breath");
            acted = true;
        }


    }
}


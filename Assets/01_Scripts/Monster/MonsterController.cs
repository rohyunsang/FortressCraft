using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class MonsterController : NetworkBehaviour
    {
        [SerializeField] public float hpMax = 3000;
        [Networked] public float Hp { get; set; }

        protected float startTime = 0.0f;
        protected float nextDelay = 0.0f;

        protected Rigidbody2D rb;
        protected Animator animator;
        protected float movingWeight = 20.0f;
        public bool ActiveSts { get; set; }
        protected bool acted = false;

        protected BodyCollider bodyCollider;
        protected MonsterHPBar hpBar;
        protected ChangeDetector changes;

        public override void Spawned()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            bodyCollider = GetComponentInChildren<BodyCollider>();
            hpBar = GetComponent<MonsterHPBar>();
            ActiveSts = false;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        }

        public virtual void MonsterAI() { }

        public bool timeCheck()
        {
            if (startTime + nextDelay > Time.fixedTime) return false;
            else return true;
        }

        public virtual void Die()
        {
            animator.SetTrigger("Die");
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }

        public virtual bool SetHP(float hp, float hpMax)
        {
            this.Hp = hp <= 0 ? 0 : hp;
            this.hpMax = hpMax;
            return (this.Hp == 0);
        }

        public void RPCCheckDamaged()
        {
            if (bodyCollider.Damaged > 0.0f)
            {
                Hp -= bodyCollider.Damaged;

                bodyCollider.Damaged = 0.0f;

                if (Hp <= 0.0f)
                {
                    Die();
                }
            }
        }

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(Hp):
                        hpBar.SetHPBar(Hp);
                        break;
                }
            }

            var interpolated = new NetworkBehaviourBufferInterpolator(this);
        }
    }
}



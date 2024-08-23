using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class MonsterController : NetworkBehaviour
    {
        [SerializeField] protected float hpMax = 3000;
        public float Hp { get; set; }

        protected float startTime = 0.0f;
        protected float nextDelay = 0.0f;

        protected Rigidbody2D rb;
        protected Animator animator;
        protected float movingWeight = 20.0f;
        public bool ActiveSts { get; set; }
        protected bool acted = false;

        protected BodyCollider bodyCollider;

        public override void Spawned()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            bodyCollider = GetComponentInChildren<BodyCollider>();
            ActiveSts = false;
        }

        public virtual void MonsterAI() { }

        public bool timeCheck()
        {
            if (startTime + nextDelay > Time.fixedTime) return false;
            else return true;
        }

        public virtual void Die()
        {
            Destroy(this.gameObject);
        }

        public virtual bool SetHP(float hp, float hpMax)
        {
            this.Hp = hp <= 0 ? 0 : hp;
            this.hpMax = hpMax;
            return (this.Hp == 0);
        }

        public void RPCCheckDamage()
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
    }
}



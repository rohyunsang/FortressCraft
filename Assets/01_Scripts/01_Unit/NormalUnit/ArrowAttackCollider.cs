using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class ArrowAttackCollider : AttackCollider
    {
        private Arrow arrow;

        private void Awake()
        {
            arrow = GetComponent<Arrow>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (TargetUnit == null) return;
            if (collision.CompareTag(TargetUnit))
            {
                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycoliider))
                {
                    bodycoliider.RPCSetDamage(Damage);
                    arrow.Release();
                }
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class ArcherArrowAttackCollider : AttackCollider
    {
        private ArcherArrow arrow;

        private void Awake()
        {
            arrow = GetComponent<ArcherArrow>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OwnType == null) return;
            if( collision.tag.StartsWith("Unit") )
            {
                if (collision.CompareTag("Unit_" + OwnType)) return;

                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycoliider))
                {
                    bodycoliider.Damaged = Damage;
                }
            }
        }
    }
}



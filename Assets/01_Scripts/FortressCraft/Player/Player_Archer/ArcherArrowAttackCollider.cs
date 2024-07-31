using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class ArcherArrowAttackCollider : AttackCollider
    {
        public override void Spawned()
        {
            base.Spawned();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OwnType == null) return;
            if( collision.tag.StartsWith("Unit") )
            {
                if (collision.CompareTag("Unit_" + OwnType))
                {
                    Debug.Log("Unit_" + OwnType);
                    return;
                }

                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycollider))
                {
                    //bodycollider.Damaged = Damage;
                    bodycollider.RPCSetDamage(Damage);
                    //Debug.Log("Damage: " + bodycollider.Damaged + " id: " + Runner.LocalPlayer.PlayerId );
                }
                
            }
        }
    }
}



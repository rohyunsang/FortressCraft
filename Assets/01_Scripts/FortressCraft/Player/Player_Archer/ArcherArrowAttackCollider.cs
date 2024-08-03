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
                    return;
                }

                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycollider))
                {
                    if ( collision.transform.parent.TryGetComponent<NormalUnitRigidBodyMovement>(
                        out NormalUnitRigidBodyMovement normal ) )
                    {
                        if( normal.HP - Damage <= 0.0f && !normal.NoReward )
                        {
                            normal.NoReward = true;
                            RewardManager.Instance.Gold += normal.gold;
                            RewardManager.Instance.Exp += normal.exp;
                        }
                    }
                    bodycollider.RPCSetDamage(Damage);
                }
                
            }
        }
    }
}



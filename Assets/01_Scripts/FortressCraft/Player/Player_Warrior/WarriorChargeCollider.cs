using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class WarriorChargeCollider : AttackCollider
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OwnType == null) return;
            if (collision.tag.StartsWith("Unit"))
            {
                if (collision.CompareTag("Unit_" + OwnType))
                {
                    return;
                }

                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycollider))
                {
                    if (collision.transform.parent != null)
                    {
                        if (collision.transform.parent.TryGetComponent<NormalUnitRigidBodyMovement>(
                        out NormalUnitRigidBodyMovement unit))
                        {
                            if (unit.HP - Damage * unit.Defense <= 0.0f && !unit.NoReward)
                            {
                                unit.NoReward = true;
                                RewardManager.Instance.Gold += unit.gold;
                                RewardManager.Instance.Exp += unit.exp;
                            }
                        }
                    }
                    bodycollider.RPCSetDamage(Damage * 2);
                }

            }
        }
    }
}


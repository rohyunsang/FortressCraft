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
                    if (collision.transform.parent.TryGetComponent<NormalUnitRigidBodyMovement>(
                        out NormalUnitRigidBodyMovement normal))
                    {
                        Debug.Log("Hit! , " + normal.HP + ", "+ Damage);
                        if (normal.HP - Damage <= 0.0f && !normal.NoReward)
                        {
                            normal.NoReward = true;
                            RewardManager.Instance.Gold += normal.gold;
                            RewardManager.Instance.Exp += normal.exp;
                        }
                    }

                    bodycoliider.RPCSetDamage(Damage);
                    arrow.Release();
                }
            }
        }
    }
}


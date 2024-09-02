using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class WarriorChargeCollider : AttackCollider
    {
        [SerializeField] private Player player;

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
                        else if (collision.transform.parent.TryGetComponent<MonsterController>(
                        out MonsterController monster))
                        {
                            if (monster.HP - Damage <= 0.0f && !monster.NoReward)
                            {
                                monster.NoReward = true;
                                RewardManager.Instance.Gold += monster.Gold;
                                RewardManager.Instance.Exp += monster.Exp;

                                if (monster.Buff == BuffType.ATTACK)
                                {
                                    player.BuffAttackTimer = TickTimer.CreateFromSeconds(Runner, player.BuffAttackTime);
                                }
                                else if (monster.Buff == BuffType.DEFENSE)
                                {
                                    player.BuffDefenseTimer = TickTimer.CreateFromSeconds(Runner, player.BuffDefenseTime);
                                }
                            }
                        }
                    }
                    bodycollider.RPCSetDamage(Damage * 2);
                    bodycollider.CallDamageCheck();
                }

            }
        }
    }
}


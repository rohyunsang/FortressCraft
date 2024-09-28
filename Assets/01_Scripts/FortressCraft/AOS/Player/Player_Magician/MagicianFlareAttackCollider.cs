using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class MagicianFlareAttackCollider : AttackCollider
    {
        public Player ClientPlayer { get; set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OwnType == null) return;
            if ( collision.tag.StartsWith("Unit") )
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
                        out NormalUnitRigidBodyMovement normal))
                        {
                            if (normal.HP - Damage * normal.Defense <= 0.0f && !normal.NoReward)
                            {
                                normal.NoReward = true;
                                RewardManager.Instance.Gold += normal.gold;
                                RewardManager.Instance.Exp += normal.exp;
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
                                    ClientPlayer.BuffAttackTimer = TickTimer.CreateFromSeconds(Runner, ClientPlayer.BuffAttackTime);
                                }
                                else if (monster.Buff == BuffType.DEFENSE)
                                {
                                    ClientPlayer.BuffDefenseTimer = TickTimer.CreateFromSeconds(Runner, ClientPlayer.BuffDefenseTime);
                                }
                            }
                        }
                    }

                    bodycollider.RPCSetDamage(Damage);
                    bodycollider.CallDamageCheck();
                }
                
            }
        }
    }
}



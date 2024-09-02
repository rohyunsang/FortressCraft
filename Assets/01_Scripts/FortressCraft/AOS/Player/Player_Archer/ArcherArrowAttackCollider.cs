using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class ArcherArrowAttackCollider : AttackCollider
    {
        public Player ClientPlayer { get; set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OwnType == null) return;
            if( collision.tag.StartsWith("Unit") )
            {
                if (collision.CompareTag("Unit_" + OwnType))
                {
                    return;
                }

                Debug.Log("Hit!");

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
                            }
                        }

                        if(collision.transform.parent.TryGetComponent<Monster_FrostLizardController>(
                        out Monster_FrostLizardController frostLizardController))
                        {
                            if (frostLizardController.HP - Damage <= 0.0f)
                            {
                                ClientPlayer.BuffAttack = TickTimer.CreateFromSeconds( Runner, ClientPlayer.BuffAttackTime );
                            }
                        }

                    }
                    //Debug.Log("Player Damage: " + Damage);
                    bodycollider.RPCSetDamage(Damage);
                    bodycollider.CallDamageCheck();
                }
                
            }
        }
    }
}



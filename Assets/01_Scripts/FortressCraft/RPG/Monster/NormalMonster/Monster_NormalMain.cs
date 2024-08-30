using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class Monster_NormalMain : NetworkBehaviour
    {
        [SerializeField] private Monster_NormalController monsterCtrl;
        [SerializeField] private int idle = 20;
        [SerializeField] private int run = 20;
        [SerializeField] private int attack = 20;
        private MonsterData monsterData;

        private int sum;
        private int num;

        private Dictionary<Monster_NormalState, float> delayDict;
        private Monster_NormalState prevState = Monster_NormalState.IDLE;
        private Monster_NormalState nextState = Monster_NormalState.IDLE;

        public override void Spawned()
        {
            monsterData = monsterCtrl.monsterData;

            delayDict = new Dictionary<Monster_NormalState, float>();
            delayDict.Add(Monster_NormalState.IDLE, monsterData.IdleDelay);
            delayDict.Add(Monster_NormalState.RUN, monsterData.RunDelay);
            delayDict.Add(Monster_NormalState.ATTACK, monsterData.AttackDelay);

            sum = idle + run + attack;

            StartCoroutine(SetMonsterState());
        }

        public IEnumerator SetMonsterState()
        {
            while (true)
            {
                prevState = nextState;
                nextState = Monster_NormalState.NON;

                if( monsterCtrl.GetNearEnemy() == null )
                {
                    nextState = Monster_NormalState.IDLE;
                }

                // 스테이트 강제 설정 ----------------------------------------------------

                if (nextState != Monster_NormalState.NON)
                {
                    monsterCtrl.SetState(nextState, delayDict[nextState]);
                    yield return new WaitForSeconds(delayDict[nextState]);
                    continue;
                }
                

                // 스테이트 일반 설정 ----------------------------------------------------
                num = Random.Range(0, sum);

                if (num < idle)
                {
                    //Debug.Log("Monster - Idle");
                    nextState = Monster_NormalState.IDLE;
                }
                else if( num < idle + run )
                {
                    nextState = Monster_NormalState.RUN;
                }
                else if( num < idle + run + attack )
                {
                    nextState = Monster_NormalState.ATTACK;
                }
                

                monsterCtrl.SetState(nextState, delayDict[nextState]);

                yield return new WaitForSeconds(delayDict[nextState]);
            }
        }
    }
}



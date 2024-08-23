using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class Monster_FrostLizardMain : NetworkBehaviour
    {
        public Monster_FrostLizardController monsterCtrl;
        public int idle = 20;
        public int walk = 20;
        public int breath = 20;

        public int sum;


        private int num;
        private int burstActualValue = 0;
        private Dictionary<Monster_FrostLizardState, float> delayDict;
        private Monster_FrostLizardState prevState = Monster_FrostLizardState.IDLE;
        private Monster_FrostLizardState nextState = Monster_FrostLizardState.IDLE;

        public override void Spawned()
        {
            delayDict = new Dictionary<Monster_FrostLizardState, float>();
            delayDict.Add(Monster_FrostLizardState.IDLE, 2.0f);
            delayDict.Add(Monster_FrostLizardState.WALK, 2.0f);
            delayDict.Add(Monster_FrostLizardState.BREATH, 3.0f);

            sum = idle + walk + breath;

            StartCoroutine(SetMonsterState());
        }

        public IEnumerator SetMonsterState()
        {
            while( true )
            {
                prevState = nextState;
                nextState = Monster_FrostLizardState.NON;

                /*
                // 스테이트 강제 설정 ----------------------------------------------------

                if (nextState != Monster_FrostLizardState.NON)
                {
                    monsterCtrl.setState(nextState, delayDict[nextState]);
                    return;
                }
                */

                // 스테이트 일반 설정 ----------------------------------------------------
                num = Random.Range(0, sum);

                if (num < idle)
                {
                    //Debug.Log("Monster - Idle");
                    nextState = Monster_FrostLizardState.IDLE;
                }
                else if (num < idle + walk) // 걷기 
                {
                    //Debug.Log("Monster - Walk");
                    nextState = Monster_FrostLizardState.WALK;
                }
                else if (num < idle + walk + breath)
                {
                    //Debug.Log("Monster - Breath");
                    nextState = Monster_FrostLizardState.BREATH;
                }

                if( prevState == nextState && nextState == Monster_FrostLizardState.BREATH )
                {
                    continue;
                }

                monsterCtrl.SetState(nextState, delayDict[nextState]);

                yield return new WaitForSeconds(delayDict[nextState]);
            }
        }
    }
}



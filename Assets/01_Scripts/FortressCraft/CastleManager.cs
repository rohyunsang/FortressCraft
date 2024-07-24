using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Agit.FortressCraft
{
    public class CastleManager : MonoBehaviour
    {
        public int defeatCnt = 0;

        public void Awake()
        {
            Init();
        }

        private void Init()
        {
            defeatCnt = 0;
        }

        public void DefeatCnt()
        {
            defeatCnt++;
            if(defeatCnt > 0)
                DefeatCntCallBack();
        }

        private void DefeatCntCallBack()
        {
            Castle[] castles = FindObjectsOfType<Castle>();

            foreach (Castle castle in castles)
            {
                castle.IsWinner();
            }
        }
    }
}
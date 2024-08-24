using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace Agit.FortressCraft
{
    public class BattleBarUIManager : NetworkBehaviour
    {
        public static BattleBarUIManager Instance { get; set; }

        public string OwnType { get; set; } // Player가 초기화 
        private Text[] unitCountTexts = new Text[4];
        private int[] unitCount = new int[4];

        private void Awake()
        {
            Instance = this;
        }

        public void SetBattleBar()
        {
            for (int i = 0; i < 4; ++i)
            {
                unitCount[i] = 0;
                unitCountTexts[i] = GameObject.Find("Contents_" + (i + 1).ToString()).gameObject.
                    GetComponentInChildren<Text>();
                unitCountTexts[i].text = "0";
            }
        } 

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCPlusUnitCount( string type )
        {
            switch( type ) {
                case "A":
                    ++unitCount[0];
                    unitCountTexts[0].text = unitCount[0].ToString();
                    break;
                case "B":
                    ++unitCount[1];
                    unitCountTexts[1].text = unitCount[1].ToString();
                    break;
                case "C":
                    ++unitCount[2];
                    unitCountTexts[2].text = unitCount[2].ToString();
                    break;
                case "D":
                    ++unitCount[3];
                    unitCountTexts[3].text = unitCount[3].ToString();
                    break;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCMinusUnitCount(string type)
        {
            switch (type)
            {
                case "A":
                    --unitCount[0];
                    unitCountTexts[0].text = unitCount[0].ToString();
                    break;
                case "B":
                    --unitCount[1];
                    unitCountTexts[1].text = unitCount[1].ToString();
                    break;
                case "C":
                    --unitCount[2];
                    unitCountTexts[2].text = unitCount[2].ToString();
                    break;
                case "D":
                    --unitCount[3];
                    unitCountTexts[3].text = unitCount[3].ToString();
                    break;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCClearUnitCount(string type)
        {
            switch (type)
            {
                case "A":
                    unitCount[0] = 0;
                    unitCountTexts[0].text = unitCount[0].ToString();
                    break;
                case "B":
                    unitCount[1] = 0;
                    unitCountTexts[1].text = unitCount[1].ToString();
                    break;
                case "C":
                    unitCount[2] = 0;
                    unitCountTexts[2].text = unitCount[2].ToString();
                    break;
                case "D":
                    unitCount[3] = 0;
                    unitCountTexts[3].text = unitCount[3].ToString();
                    break;
            }
        }
    }
}


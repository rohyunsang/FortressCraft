using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Agit.FortressCraft
{
    public class Enhancement : MonoBehaviour
    {
        private Player player;

        // 증강 버튼
        private Button[] btn = new Button[3];

        private Text[] btnRankText = new Text[3];
        private Text[] btnNameText = new Text[3];
        private Text[] btnUpgradeText = new Text[3];

        private delegate void EnhanceFunc();
        private List<EnhanceFunc> EnhanceList;

        private int enhanceLength;

        public void Init()
        {
            btn[0] = GameObject.Find("EnhBtn1").GetComponent<Button>();
            btnRankText[0] = btn[0].transform.Find("Texts/RankText").GetComponent<Text>();
            btnNameText[0] = btn[0].transform.Find("Texts/NameText").GetComponent<Text>();
            btnUpgradeText[0] = btn[0].transform.Find("Image/UpgradeText").GetComponent<Text>();

            btn[1] = GameObject.Find("EnhBtn2").GetComponent<Button>();
            btnRankText[1] = btn[1].transform.Find("Texts/RankText").GetComponent<Text>();
            btnNameText[1] = btn[1].transform.Find("Texts/NameText").GetComponent<Text>();
            btnUpgradeText[1] = btn[1].transform.Find("Image/UpgradeText").GetComponent<Text>();

            btn[2] = GameObject.Find("EnhBtn3").GetComponent<Button>();
            btnRankText[2] = btn[2].transform.Find("Texts/RankText").GetComponent<Text>();
            btnNameText[2] = btn[2].transform.Find("Texts/NameText").GetComponent<Text>();
            btnUpgradeText[2] = btn[2].transform.Find("Image/UpgradeText").GetComponent<Text>();

            player = GetComponent<Player>();
            EnhanceList = new List<EnhanceFunc>();

            EnhanceList.Add(Enhance1);
            EnhanceList.Add(Enhance2);
            EnhanceList.Add(Enhance3);
            EnhanceList.Add(Enhance4);

            enhanceLength = EnhanceList.Count;
        }

        public void EnhancementSetting()
        {
            for( int i = 0; i < 3; ++i )
            {
                int idx = Random.Range(0, enhanceLength);
                btn[i].onClick.RemoveAllListeners();
                btn[i].onClick.AddListener(() => EnhanceList[idx]());
                TextSetting(i, idx);
            }
        }

        private void TextSetting(int btnNum, int enhanceNum)
        {
            switch(enhanceNum)
            {
                case 1:
                    btnRankText[btnNum].text = "일반";
                    btnNameText[btnNum].text = "돈 주고도 못 사~";
                    btnUpgradeText[btnNum].text = "경험치 30 획득!";
                    break;
                case 2:
                    btnRankText[btnNum].text = "일반";
                    btnNameText[btnNum].text = "배부르게 먹자";
                    btnUpgradeText[btnNum].text = "체력 +100, 골드 + 40";
                    break;
                case 3:
                    btnRankText[btnNum].text = "일반";
                    btnNameText[btnNum].text = "수색조";
                    btnUpgradeText[btnNum].text = "유닛 속도 5% 증가";
                    break;
            }
        }

        private void Enhance1()
        {
            RewardManager.Instance.Exp += 130;
        }

        private void Enhance2()
        {
            player.life += 100;
            RewardManager.Instance.Gold += 40;
        }

        private void Enhance3()
        {
            NormalUnitDataManager.Instance.Speed *= 1.05f;
        }

        private void Enhance4()
        {
            Debug.Log("Enhance - 4");
        }
    }
}


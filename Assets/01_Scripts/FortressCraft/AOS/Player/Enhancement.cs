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

        [SerializeField] private int enhance1weight = 20;
        [SerializeField] private int enhance2weight = 20;
        [SerializeField] private int enhance3weight = 20;
        [SerializeField] private int enhance4weight = 20;
        [SerializeField] private int enhance5weight = 20;
        [SerializeField] private int enhance6weight = 10;
        [SerializeField] private int enhance7weight = 10;
        [SerializeField] private int enhance8weight = 10;

        private int sumOfWeight;

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
            EnhanceList.Add(Enhance5);
            EnhanceList.Add(Enhance6);
            EnhanceList.Add(Enhance7);
            EnhanceList.Add(Enhance8);

            enhanceLength = EnhanceList.Count;
            sumOfWeight = enhance1weight + enhance2weight + enhance3weight + enhance4weight
                + enhance5weight + enhance6weight + enhance7weight + enhance8weight;
        }

        public void EnhancementSetting()
        {
            for( int i = 0; i < 3; ++i )
            {
                int idx = Random.Range(0, sumOfWeight);
                btn[i].onClick.RemoveAllListeners();
                btn[i].onClick.AddListener(() => EnhanceList[idx]());
                TextSetting(i, idx);
            }
        }

        private void TextSetting(int btnNum, int enhanceNum)
        {
            if( enhanceNum < enhance1weight )
            {
                btnRankText[btnNum].text = "일반";
                btnNameText[btnNum].text = "돈 주고도 못 사~";
                btnUpgradeText[btnNum].text = "경험치 +30";
            }
            else if( enhanceNum < enhance1weight + enhance2weight )
            {
                btnRankText[btnNum].text = "일반";
                btnNameText[btnNum].text = "배부르게 먹자";
                btnUpgradeText[btnNum].text = "체력 +100\n골드 + 40";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight)
            {
                btnRankText[btnNum].text = "일반";
                btnNameText[btnNum].text = "수색조";
                btnUpgradeText[btnNum].text = "유닛 이동속도\n10% 증가\n\n유닛 공격속도\n5% 감소";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight + enhance4weight)
            {
                btnRankText[btnNum].text = "일반";
                btnNameText[btnNum].text = "월급날 발걸음";
                btnUpgradeText[btnNum].text = "유닛 이동속도\n5% 증가\n\n골드 + 40";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight + enhance4weight
                + enhance5weight)
            {
                btnRankText[btnNum].text = "일반";
                btnNameText[btnNum].text = "돈으로 해결";
                btnUpgradeText[btnNum].text = "유닛 공격속도\n5% 증가\n\n골드 + 20";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight + enhance4weight
                + enhance5weight + enhance6weight)
            {
                btnRankText[btnNum].text = "희귀";
                btnNameText[btnNum].text = "사기증진";
                btnUpgradeText[btnNum].text = "유닛 공격속도\n20% 증가";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight + enhance4weight
                + enhance5weight + enhance6weight + enhance7weight)
            {
                btnRankText[btnNum].text = "희귀";
                btnNameText[btnNum].text = "하체 단련";
                btnUpgradeText[btnNum].text = "유닛 이동속도\n20% 증가";
            }
            else if (enhanceNum < enhance1weight + enhance2weight + enhance3weight + enhance4weight
                + enhance5weight + enhance6weight + enhance7weight + enhance8weight)
            {
                btnRankText[btnNum].text = "희귀";
                btnNameText[btnNum].text = "돈이 최고야!";
                btnUpgradeText[btnNum].text = "골드 +100";
            }
        }

        private void Enhance1()
        {
            RewardManager.Instance.Exp += 30;
        }

        private void Enhance2()
        {
            player.life += 100;
            RewardManager.Instance.Gold += 40;
        }

        private void Enhance3()
        {
            NormalUnitDataManager.Instance.Speed *= 1.1f;
            NormalUnitDataManager.Instance.AttackDelay *= 0.95f;
        }

        private void Enhance4()
        {
            NormalUnitDataManager.Instance.Speed *= 1.05f;
            RewardManager.Instance.Gold += 40;
        }

        private void Enhance5()
        {
            NormalUnitDataManager.Instance.AttackDelay *= 1.05f;
            RewardManager.Instance.Gold += 20;
        }

        private void Enhance6()
        {
            NormalUnitDataManager.Instance.AttackDelay *= 1.2f;
        }

        private void Enhance7()
        {
            NormalUnitDataManager.Instance.Speed *= 1.2f;
        }

        private void Enhance8()
        {
            RewardManager.Instance.Gold += 100;
        }
    }
}


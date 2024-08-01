using Fusion;
using UnityEngine;
using UnityEngine.UI;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class ChangeTarget : NetworkBehaviour
    {
        private NormalUnitSpawner[] spawners;
        private NormalUnitSpawner targetSpawner;
        private Button[] btns;
        private Button attackBtn;
        private Color orgColor;
        private Color unselectedColor;
        private bool isAttackOn = true;

        private string ownType;

        private string buttonNameA = "Button_1";
        private string buttonNameB = "Button_2";
        private string buttonNameC = "Button_3";
        private string buttonNameD = "Button_4";
        private string buttonNameAttack = "Button_Attack";

        private void Awake()
        {
            btns = GetComponentsInChildren<Button>();
            
            foreach (Button btn in btns)
            {
                if (btn.transform.name == buttonNameA )
                {
                    btn.onClick.AddListener(SetTargetA);
                }
                else if (btn.transform.name == buttonNameB )
                {
                    btn.onClick.AddListener(SetTargetB);
                }
                else if (btn.transform.name == buttonNameC )
                {
                    btn.onClick.AddListener(SetTargetC);
                }
                else if (btn.transform.name == buttonNameD )
                {
                    btn.onClick.AddListener(SetTargetD);
                }
                else if (btn.transform.name == buttonNameAttack )
                {
                    attackBtn = btn;
                    btn.onClick.AddListener(SetAttack);
                }
            }
            /*
            int idx = -1;
            if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
            {
                idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);

                switch (idx)
                {
                    case 0:
                        ownType = "A";
                        break;
                    case 1:
                        ownType = "B";
                        break;
                    case 2:
                        ownType = "C";
                        break;
                    case 3:
                        ownType = "D";
                        break;
                }
            }
            */
            //orgColor = btns[0].GetComponent<Image>().color;
            //unselectedColor = new Color(0.5f, 0.5f, 0.5f);
        }

        // 골라진 타겟만 색을 바꾸도록 설정 
        private void UpdateTargetButtonColor(string name)
        {
            foreach( Button btn in btns )
            {
                if (btn.transform.name == buttonNameAttack) continue;

                if( btn.transform.name == name )
                {
                    btn.GetComponent<Image>().color = orgColor;
                }
                else
                {
                    btn.GetComponent<Image>().color = unselectedColor;
                }
            }
        }

        private void UpdateAttackButtonColor()
        {
            if( isAttackOn )
            {
                attackBtn.GetComponent<Image>().color = unselectedColor;
            }
            else
            {
                attackBtn.GetComponent<Image>().color = orgColor;
            }

            isAttackOn = !isAttackOn;
        }

        public void SetTargetA()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(ownType) != 0) continue;

                Debug.Log("Set as A");
                spawner.RPCTargetChange("A");
                UpdateTargetButtonColor(buttonNameA);
            }
            /*
            if (targetSpawner == null) return;
            Debug.Log("Set as A");
            targetSpawner.RPCTargetChange("A");
            UpdateTargetButtonColor(buttonNameA);
            */
        }

        public void SetTargetB()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(ownType) != 0) continue;

                Debug.Log("Set as B");
                spawner.RPCTargetChange("B");
                UpdateTargetButtonColor(buttonNameB);
            }
            /*
            if (targetSpawner == null) return;
            Debug.Log("Set as B");
            targetSpawner.RPCTargetChange("B");
            UpdateTargetButtonColor(buttonNameB);
            */
        }

        public void SetTargetC()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(ownType) != 0) continue;

                Debug.Log("Set as C");
                spawner.RPCTargetChange("C");
                UpdateTargetButtonColor(buttonNameC);
            }
            /*
            if (targetSpawner == null) return;
            Debug.Log("Set as C");
            targetSpawner.RPCTargetChange("C");
            UpdateTargetButtonColor(buttonNameC);
            */
        }

        public void SetTargetD()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach( NormalUnitSpawner spawner in spawners )
            {
                if (spawner.SpawnerType.CompareTo(ownType) != 0) continue;

                Debug.Log("Set as D");
                spawner.RPCTargetChange("D");
                UpdateTargetButtonColor(buttonNameD);
            }

            /*
            if (targetSpawner == null) return;
            Debug.Log("Set as D");
            targetSpawner.RPCTargetChange("D");
            UpdateTargetButtonColor(buttonNameD);
            */
        }

        public void SetAttack()
        {
            /*
            targetSpawner.RPCChangeAttackEnabled();
            UpdateAttackButtonColor();
            */
        }

    }

}
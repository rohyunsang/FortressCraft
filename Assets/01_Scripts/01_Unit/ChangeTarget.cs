using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class ChangeTarget : NetworkBehaviour
    {
        private NormalUintSpawner[] spawners;
        private NormalUintSpawner targetSpawner;
        private Button[] btns;
        private Button attackBtn;
        private Color orgColor;
        private Color unselectedColor;
        private bool isAttackOn = true;

        private string buttonNameA = "Button_A";
        private string buttonNameB = "Button_B";
        private string buttonNameC = "Button_C";
        private string buttonNameD = "Button_D";
        private string buttonNameAttack = "Button_Attack";

        private void Awake()
        {
            spawners = GameObject.FindObjectsOfType<NormalUintSpawner>();
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

            orgColor = btns[0].GetComponent<Image>().color;
            unselectedColor = new Color(0.5f, 0.5f, 0.5f);
        }

        private void FixedUpdate()
        {
            if (targetSpawner == null) SettingSpawner();
        }

        private void SettingSpawner()
        {
            foreach (NormalUintSpawner spawner in spawners)
            {
                if (spawner.player == null) continue;

                if (spawner.player.PlayerId.PlayerId == Runner.LocalPlayer.PlayerId)
                {
                    targetSpawner = spawner;
                    break;
                }
            }

            switch( targetSpawner.Target )
            {
                case "A":
                    UpdateTargetButtonColor(buttonNameA);
                    break;
                case "B":
                    UpdateTargetButtonColor(buttonNameB);
                    break;
                case "C":
                    UpdateTargetButtonColor(buttonNameC);
                    break;
                case "D":
                    UpdateTargetButtonColor(buttonNameD);
                    break;
            }
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
            /*
            if(targetSpawner.AttackEnabled)
            {
                attackBtn.GetComponent<Image>().color = orgColor;
            }
            else
            {
                attackBtn.GetComponent<Image>().color = unselectedColor;
            }
            */
        }

        public void SetTargetA()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as A");
            targetSpawner.RPCTargetChange("A");
            UpdateTargetButtonColor(buttonNameA);
        }

        public void SetTargetB()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as B");
            targetSpawner.RPCTargetChange("B");
            UpdateTargetButtonColor(buttonNameB);
        }

        public void SetTargetC()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as C");
            targetSpawner.RPCTargetChange("C");
            UpdateTargetButtonColor(buttonNameC);
        }

        public void SetTargetD()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as D");
            targetSpawner.RPCTargetChange("D");
            UpdateTargetButtonColor(buttonNameD);
        }

        public void SetAttack()
        {
            targetSpawner.RPCChangeAttackEnabled();
            UpdateAttackButtonColor();
        }

    }

}
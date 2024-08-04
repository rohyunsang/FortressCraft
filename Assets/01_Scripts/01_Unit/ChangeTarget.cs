using Fusion;
using UnityEngine;
using UnityEngine.UI;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class ChangeTarget : NetworkBehaviour
    {
        private NormalUnitSpawner[] spawners;
        private Button[] btns;
        private Button attackBtn;
        private Color orgColor;
        private Color unselectedColor;
        public bool IsAttackOn { get; set; }

        public string OwnType { get; set; }
        public string Target { get; set; }

        private string buttonNameA = "Button_1";
        private string buttonNameB = "Button_2";
        private string buttonNameC = "Button_3";
        private string buttonNameD = "Button_4";
        private string buttonNameAttack = "Button_Attack";

        private void Awake()
        {
            IsAttackOn = true;
            Target = null;
            OwnType = "";
            btns = GetComponentsInChildren<Button>();
            Debug.Log("Change Target Start");
            foreach (Button btn in btns)
            {
                if (btn.transform.name == buttonNameA)
                {
                    btn.onClick.AddListener(SetTargetA);
                }
                else if (btn.transform.name == buttonNameB)
                {
                    btn.onClick.AddListener(SetTargetB);
                }
                else if (btn.transform.name == buttonNameC)
                {
                    btn.onClick.AddListener(SetTargetC);
                }
                else if (btn.transform.name == buttonNameD)
                {
                    btn.onClick.AddListener(SetTargetD);
                }
                else if (btn.transform.name == buttonNameAttack)
                {
                    attackBtn = btn;
                    btn.onClick.AddListener(SetAttack);
                }
            }

            orgColor = btns[0].GetComponent<Image>().color;
            unselectedColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        }

        // 골라진 타겟만 색을 바꾸도록 설정 
        public void UpdateTargetButtonColor(string name)
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
            if( IsAttackOn )
            {
                attackBtn.GetComponent<Image>().color = unselectedColor;
            }
            else
            {
                attackBtn.GetComponent<Image>().color = orgColor;
            }

            IsAttackOn = !IsAttackOn;
        }

        public void SetTargetA()
        {
            Debug.Log("Owntype: " + OwnType);
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();
            Target = "A";

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Spawner: " + spawner.SpawnerType + " . " + OwnType);
                Debug.Log("Set as A");
                spawner.RPCTargetChange("A");
                UpdateTargetButtonColor(buttonNameA);
            }
        }

        public void SetTargetB()
        {
            Debug.Log("Owntype: " + OwnType);
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();
            Target = "B";

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Spawner: " + spawner.SpawnerType + " . " + OwnType);
                Debug.Log("Set as B");
                spawner.RPCTargetChange("B");
                UpdateTargetButtonColor(buttonNameB);
            }
        }

        public void SetTargetC()
        {
            Debug.Log("Owntype: " + OwnType);
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();
            Target = "C";

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Spawner: " + spawner.SpawnerType + " . " + OwnType);
                Debug.Log("Set as C");
                spawner.RPCTargetChange("C");
                UpdateTargetButtonColor(buttonNameC);
            }
        }

        public void SetTargetD()
        {
            Debug.Log("Owntype: " + OwnType);
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();
            Target = "D";

            foreach( NormalUnitSpawner spawner in spawners )
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Spawner: " + spawner.SpawnerType + " . " + OwnType);
                Debug.Log("Set as D");
                spawner.RPCTargetChange("D");
                UpdateTargetButtonColor(buttonNameD);
            }
        }

        public void SetAttack()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Attack Setting Changed");
                spawner.RPCChangeAttackEnabled();
                UpdateAttackButtonColor();
            }
        }
    }
}
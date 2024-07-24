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

        private void Awake()
        {
            spawners = GameObject.FindObjectsOfType<NormalUintSpawner>();
            btns = GetComponentsInChildren<Button>();

            foreach (Button btn in btns)
            {
                if (btn.transform.name == "Button_A")
                {
                    btn.onClick.AddListener(setTargetA);
                }
                else if (btn.transform.name == "Button_B")
                {
                    btn.onClick.AddListener(setTargetB);
                }
                else if (btn.transform.name == "Button_C")
                {
                    btn.onClick.AddListener(setTargetC);
                }
                else if (btn.transform.name == "Button_D")
                {
                    btn.onClick.AddListener(setTargetD);
                }
                else if (btn.transform.name == "Button_Attack")
                {
                    btn.onClick.AddListener(SetAttack);
                }
            }
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
        }

        public void setTargetA()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as A");
            targetSpawner.RPCTargetChange("A");
        }


        public void setTargetB()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as B");
            targetSpawner.RPCTargetChange("B");
        }

        public void setTargetC()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as C");
            targetSpawner.RPCTargetChange("C");
        }

        public void setTargetD()
        {
            if (targetSpawner == null) return;
            Debug.Log("Set as D");
            targetSpawner.RPCTargetChange("D");
        }

        public void SetAttack()
        {
            if (targetSpawner == null) return;

            if (targetSpawner.AttackEnabled == true)
            {
                OffAttack();
            }
            else
            {
                OnAttack();
            }
        }

        private void OffAttack()
        {
            Debug.Log("Off Attack");
            targetSpawner.RPCSettingAttackEnabled("Off");
        }

        private void OnAttack()
        {
            Debug.Log("On Attack");
            targetSpawner.RPCSettingAttackEnabled("On");
        }
    }

}
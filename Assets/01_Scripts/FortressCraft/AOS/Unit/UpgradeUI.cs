using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Text attackLevel;
        [SerializeField] private Text defenseLevel;
        [SerializeField] private Text timeLevel;
        [SerializeField] private Text attackValue;
        [SerializeField] private Text defenseValue;
        [SerializeField] private Text timeValue;
        [SerializeField] private Text attackCost;
        [SerializeField] private Text defenseCost;
        [SerializeField] private Text timeCost;

        // idx: 0 -> attack, 1 -> defense, 2 -> time
        public void SetUpgradeUIText(int idx, int level)
        {
            UnitData unitData = GoogleSheetManager.GetUnitData(level);
            switch (idx)
            {
                case 0:
                    attackLevel.text = "Lv " + level.ToString();
                    attackValue.text = unitData.Attack.ToString();
                    attackCost.text = unitData.UpgradeCost.ToString();
                    break;
                case 1:
                    defenseLevel.text = "Lv " + level.ToString();
                    defenseValue.text = unitData.Defense.ToString();
                    defenseCost.text = unitData.UpgradeCost.ToString();
                    break;
                case 2:
                    timeLevel.text = "Lv " + level.ToString();
                    timeValue.text = unitData.SpawnDelay.ToString() + "초";
                    timeCost.text = unitData.UpgradeCost.ToString();
                    break;
            }
        }
    }
}



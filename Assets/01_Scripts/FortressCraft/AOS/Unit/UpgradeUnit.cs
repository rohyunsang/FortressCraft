using Fusion;
using UnityEngine;
using UnityEngine.UI;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class UpgradeUnit : NetworkBehaviour
    {
        public string OwnType { get; set; }

        private NormalUnitSpawner[] spawners;
        private NormalUnitSpawner targetSpawner;
        private Button btnAttackUpgrade;
        private Button btnDefenseUpgrade;
        private Button btnTimeUpgrade;

        private Image[] btnAttackUpgradeImages;
        private Image[] btnDefenseUpgradeImages;
        private Image[] btnTimeUpgradeImages;

        private int attackLevel = 1;
        private int defenseLevel = 1;
        private int timeLevel = 1;
        private int attackLevelLimit = 11;
        private int defenseLevelLimit = 11;
        private int timeLevelLimit = 11;

        private UpgradeUI upgradeUI;

        private void Awake()
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (player.Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
            {
                int idx = gameManager.TryGetPlayerId(player.Runner.LocalPlayer);
                Debug.Log("idx: " + idx);
                switch (idx)
                {
                    case 0:
                        OwnType = "A";
                        break;
                    case 1:
                        OwnType = "B";
                        break;
                    case 2:
                        OwnType = "C";
                        break;
                    case 3:
                        OwnType = "D";
                        break;
                }
            }

            btnAttackUpgrade = transform.Find("AttackGroup").GetComponentInChildren<Button>();
            btnAttackUpgrade.onClick.AddListener(UpgradeDamage);
            btnAttackUpgradeImages = transform.Find("AttackGroup").GetComponentsInChildren<Image>();

            btnDefenseUpgrade = transform.Find("DefensiveGroup").GetComponentInChildren<Button>();
            btnDefenseUpgrade.onClick.AddListener(UpgradeDefense);
            btnDefenseUpgradeImages = transform.Find("DefensiveGroup").GetComponentsInChildren<Image>();

            btnTimeUpgrade = transform.Find("SummoningTimeGroup").GetComponentInChildren<Button>();
            btnTimeUpgrade.onClick.AddListener(UpgradeSpawnTime);
            btnTimeUpgradeImages = transform.Find("SummoningTimeGroup").GetComponentsInChildren<Image>();

            upgradeUI = GetComponent<UpgradeUI>();
            upgradeUI.SetUpgradeUIText(0, 1);
            upgradeUI.SetUpgradeUIText(1, 1);
            upgradeUI.SetUpgradeUIText(2, 1);
        }

        public void UpgradeDamage()
        {
            if (attackLevel >= attackLevelLimit) return;

            UnitData unitData = GoogleSheetManager.GetUnitData(attackLevel);

            if (RewardManager.Instance.Gold < unitData.UpgradeCost ) return;
            RewardManager.Instance.Gold -= unitData.UpgradeCost;

            ++attackLevel;

            unitData = GoogleSheetManager.GetUnitData(attackLevel);
            NormalUnitDataManager.Instance.Attack = unitData.Attack;

            upgradeUI.SetUpgradeUIText(0, attackLevel);

            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Damage : " + unitData.Attack);
                spawner.RPCSettingDamage( unitData.Attack );
            }

            if( attackLevel == attackLevelLimit )
            {
                foreach (Image image in btnAttackUpgradeImages)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
                }
            }
        }

        public void UpgradeDefense()
        {
            if (defenseLevel >= defenseLevelLimit) return;

            UnitData unitData = GoogleSheetManager.GetUnitData(defenseLevel);

            if (RewardManager.Instance.Gold < unitData.Defense ) return;
            RewardManager.Instance.Gold -= unitData.UpgradeCost;

            ++defenseLevel;

            upgradeUI.SetUpgradeUIText(1, defenseLevel);

            unitData = GoogleSheetManager.GetUnitData(defenseLevel);
            NormalUnitDataManager.Instance.Defense = unitData.Defense;

            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Defense - " + unitData.Defense);
                spawner.RPCSettingDefense(unitData.Defense);
            }

            if( defenseLevel == defenseLevelLimit )
            {
                foreach (Image image in btnDefenseUpgradeImages)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
                }
            }
        }

        public void UpgradeSpawnTime()
        {
            if (timeLevel >= timeLevelLimit) return;

            UnitData unitData = GoogleSheetManager.GetUnitData(timeLevel);

            if (RewardManager.Instance.Gold < unitData.UpgradeCost) return;
            RewardManager.Instance.Gold -= unitData.UpgradeCost;

            ++timeLevel;

            upgradeUI.SetUpgradeUIText(2, timeLevel);

            unitData = GoogleSheetManager.GetUnitData(timeLevel);
            NormalUnitDataManager.Instance.SpawnTime = unitData.SpawnDelay;

            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;

                NormalUnitGenerator generator = spawner.GetComponent<NormalUnitGenerator>();
                generator.SpawnTime = unitData.SpawnDelay;
            }

            if (timeLevel == timeLevelLimit)
            {
                foreach (Image image in btnTimeUpgradeImages)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
                }
            }
        }
    }
}
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class UpgradeUnit : NetworkBehaviour
    {
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
        private int attackLevelLimit = 2;
        private int defenseLevelLimit = 2;
        private int timeLevelLimit = 2;

        [SerializeField] private int attackCost = 50;
        [SerializeField] private int defenseCost = 50;
        [SerializeField] private int summonTimeCost = 500;

        public string OwnType { get; set; }

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
        }

        public void UpgradeDamage()
        {
            if (attackLevel >= attackLevelLimit) return;
            
            if (RewardManager.Instance.Gold < attackCost) return;
            RewardManager.Instance.Gold -= attackCost;
            ++attackLevel;
            NormalUnitDataManager.Instance.Attack = 100.0f;

            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Damage");
                spawner.RPCSettingDamage(100.0f);
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
            
            if (RewardManager.Instance.Gold < defenseCost) return;
            RewardManager.Instance.Gold -= defenseCost;
            ++defenseLevel;
            NormalUnitDataManager.Instance.Defense = 0.5f;

            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Defense");
                spawner.RPCSettingDefense(0.5f);
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

        }
    }
}
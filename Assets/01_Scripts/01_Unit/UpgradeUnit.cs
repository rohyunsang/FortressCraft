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

            btnDefenseUpgrade = transform.Find("DefensiveGroup").GetComponentInChildren<Button>();
            btnDefenseUpgrade.onClick.AddListener(UpgradeDefense);

            btnTimeUpgrade = transform.Find("SummoningTimeGroup").GetComponentInChildren<Button>();
            btnTimeUpgrade.onClick.AddListener(UpgradeSpawnTime);
        }

        public void UpgradeDamage()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Damage");
                spawner.RPCSettingDamage(100.0f);
            }
        }

        public void UpgradeDefense()
        {
            spawners = GameObject.FindObjectsOfType<NormalUnitSpawner>();

            foreach (NormalUnitSpawner spawner in spawners)
            {
                if (spawner.SpawnerType.CompareTo(OwnType) != 0) continue;
                Debug.Log("Upgrade Defense");
                spawner.RPCSettingDefense(0.5f);
            }
        }

        public void UpgradeSpawnTime()
        {

        }
    }
}
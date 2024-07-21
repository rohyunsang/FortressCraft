using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUnit : NetworkBehaviour
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
            if (btn.transform.name == "Upgrade_Damage")
            {
                btn.onClick.AddListener(UpgradeDamage);
            }
            else if (btn.transform.name == "Upgrade_Defense")
            {
                btn.onClick.AddListener(UpgradeDefense);
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

    public void UpgradeDamage()
    {
        if (targetSpawner == null) return;
        Debug.Log("Upgrade Damage");
        targetSpawner.RPCSettingDamage(100.0f);
    }

    public void UpgradeDefense()
    {
        if (targetSpawner == null) return;
        Debug.Log("Upgrade Defense");
        targetSpawner.RPCSettingDamage(1.0f);
    }
}

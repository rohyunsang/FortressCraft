using UnityEngine;
using Fusion;
using Agit.FortressCraft;


public class NormalUintSpawner : NetworkBehaviour
{
    public NetworkPrefabRef UnitPrefab;
    public Player player;
    public bool Usable { get; private set; }

    [SerializeField] private string initialTarget = "";

    // RPC property
    public string Target { get; set; }
    public bool AttackEnabled { get; set; }
    public float Damage { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCTargetChange(string t)
    {
        Debug.Log("t");
        Target = t;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCSettingAttackEnabled(string s)
    {
        if (s == "Off") {
            Debug.Log("Change AttackEnabled - Off");
            AttackEnabled = false;
        }
        else
        {
            Debug.Log("Change AttackEnabled - On");
            AttackEnabled = true;
        }
    }

    public override void Spawned()
    {
        Usable = false;

        Target = initialTarget;
        AttackEnabled = true;
        Damage = 100.0f;

        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player p in players)
        {
            if (p.PlayerIndex == 0 && Target == "B" ||
                p.PlayerIndex == 1 && Target == "C" ||
                p.PlayerIndex == 2 && Target == "D" ||
                p.PlayerIndex == 3 && Target == "A")
            {
                player = p;
                Usable = true;
            }
        }
    }

    public void SpawnUnit(NetworkRunner runner)
    {   
        if (runner.IsSharedModeMasterClient)
        {
            NetworkObject unitObj = runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
            NormalUnitRigidBodyMovement normalUnitRigidBodyMovement = unitObj.GetComponent<NormalUnitRigidBodyMovement>();
            
            normalUnitRigidBodyMovement.TargetString = Target;
            normalUnitRigidBodyMovement.AttackEnabled = AttackEnabled;
            normalUnitRigidBodyMovement.Damage = Damage;

            normalUnitRigidBodyMovement.Spawner = this;
            normalUnitRigidBodyMovement.Initializing();
        }
    }
}

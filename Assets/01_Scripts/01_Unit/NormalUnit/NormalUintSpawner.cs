using UnityEngine;
using Fusion;
using Agit.FortressCraft;


public class NormalUintSpawner : NetworkBehaviour
{
    public NetworkPrefabRef UnitPrefab;
    public Player player;
    public bool Usable { get; private set; }

    [SerializeField] private string initialTarget = "";
    public string Target { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCTargetChange(string t)
    {
        Target = t;
    }

    public override void Spawned()
    {
        Usable = false;
        Target = initialTarget;

        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player p in players)
        {
            if (p.PlayerIndex == 0 && Target == "A" ||
                p.PlayerIndex == 1 && Target == "B" ||
                p.PlayerIndex == 2 && Target == "C" ||
                p.PlayerIndex == 3 && Target == "D")
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
            //Debug.Log(unitObj);
            
            normalUnitRigidBodyMovement.TargetString = Target;
            normalUnitRigidBodyMovement.Spawner = this;
            normalUnitRigidBodyMovement.Initializing();
        }
    }
}

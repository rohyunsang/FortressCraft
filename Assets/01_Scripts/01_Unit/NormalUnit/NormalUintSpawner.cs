using UnityEngine;
using Fusion;
using Agit.FortressCraft;
using FusionHelpers;

public class NormalUintSpawner : NetworkBehaviour
{
    public NetworkObject UnitPrefab;
    public Player player = null;
    public bool Usable { get; private set; }
    public NetworkObjectPoolManager poolManager;
    [SerializeField] private string initialTarget = "";

    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxPoolSize = 10;

    // RPC property
    public string Target { get; set; }
    public bool AttackEnabled { get; set; }
    public float Damage { get; set; }
    public float Defense { get; set; }

    public NetworkPrefabId id;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCTargetChange(string t)
    {
        Debug.Log("target: " + t);
        Target = t;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCSettingAttackEnabled(string s)
    {
        if (s == "Off")
        {
            Debug.Log("Change AttackEnabled - Off");
            AttackEnabled = false;
        }
        else
        {
            Debug.Log("Change AttackEnabled - On");
            AttackEnabled = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCSettingDamage( float newDefense )
    {
        Damage = newDefense;
        Debug.Log("Defense: " + newDefense);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCSettingDefense(float newDamage)
    {
        Damage = newDamage;
        Debug.Log("Damage: " + Damage);
    }

    public override void Spawned()
    {
        Usable = false;

        //GetComponent<NetworkObject>().RequestStateAuthority();
        poolManager = GetComponent<NetworkObjectPoolManager>();
        Target = initialTarget;
        AttackEnabled = true;
        Damage = 20.0f;
        Defense = 0.5f;

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

        NetworkObject temp = Runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
        id = temp.NetworkTypeId.AsPrefabId;
        Destroy(temp.gameObject);
        poolManager.AddPoolTable(id);
    }

    public void SpawnUnit()
    {
        
        if (Runner.IsSharedModeMasterClient)
        {
            NetworkObject unitObj = null;
            //NetworkPrefabId id = NetworkPrefabId.FromIndex(2);
            
            NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
            var result = poolManager.AcquirePrefabInstance(Runner, context, out unitObj);

            if(result == NetworkObjectAcquireResult.Success)
            {
                NormalUnitRigidBodyMovement normalUnitRigidBodyMovement = unitObj.GetComponent<NormalUnitRigidBodyMovement>();

                normalUnitRigidBodyMovement.TargetString = Target;
                normalUnitRigidBodyMovement.AttackEnabled = AttackEnabled;
                normalUnitRigidBodyMovement.Damage = Damage;
                normalUnitRigidBodyMovement.Defense = Defense;

                normalUnitRigidBodyMovement.Spawner = this;
                normalUnitRigidBodyMovement.IsActive = true;
                normalUnitRigidBodyMovement.Initializing();
            }
            else
            {
                Debug.LogError("Pooling Failed");
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPCSetActive(NetworkObject NO)
    {
        NO.gameObject.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPCSetUnactive(NetworkObject NO)
    {
        NO.gameObject.SetActive(false);
    }
}

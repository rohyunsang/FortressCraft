using UnityEngine;
using Fusion;
using UnityEngine.Pool;
using Agit.FortressCraft;


public class NormalUintSpawner : NetworkBehaviour
{
    public NetworkPrefabRef UnitPrefab;
    public Player player = null;
    public bool Usable { get; private set; }

    [SerializeField] private string initialTarget = "";

    public IObjectPool<GameObject> Pool { get; set; }
    [SerializeField] private int unitCapacity = 5;
    [SerializeField] private int maxPoolSize = 10;

    // RPC property
    public string Target { get; set; }
    public bool AttackEnabled { get; set; }
    public float Damage { get; set; }
    public float Defense { get; set; }

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

        GetComponent<NetworkObject>().RequestStateAuthority();

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

        if (player != null) {
            InitPooling();
        }
    }

    public void SpawnUnit(NetworkRunner runner)
    {
        /*
        if (runner.IsSharedModeMasterClient)
        {
            NetworkObject unitObj = runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
            NormalUnitRigidBodyMovement normalUnitRigidBodyMovement = unitObj.GetComponent<NormalUnitRigidBodyMovement>();
            
            normalUnitRigidBodyMovement.TargetString = Target;
            normalUnitRigidBodyMovement.AttackEnabled = AttackEnabled;
            normalUnitRigidBodyMovement.Damage = Damage;
            normalUnitRigidBodyMovement.Defense = Defense;

            normalUnitRigidBodyMovement.Spawner = this;
            normalUnitRigidBodyMovement.Initializing();
        }
        */
        if(runner.IsSharedModeMasterClient)
        {
            Pool.Get();
        }
    }
    
    // Object Pooling
    private void InitPooling()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        Pool = new ObjectPool<GameObject>(CreatPooledItem, OnTakeFromPool,
                                             OnReturnedToPool, OnDestroyPoolObject,
                                             true, unitCapacity, maxPoolSize);

        // 미리 생성
        for( int i = 0; i < unitCapacity; ++i )
        {
            GameObject no = CreatPooledItem();
            NormalUnitRigidBodyMovement normal = no.GetComponent<NormalUnitRigidBodyMovement>();
            normal._Pool.Release(no);
        }
    }

    // 생성 
    private GameObject CreatPooledItem()
    {
        NetworkObject unitObj = null;
        if (Runner.IsSharedModeMasterClient)
        {
            unitObj = Runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
            NormalUnitRigidBodyMovement normalUnitRigidBodyMovement = unitObj.GetComponent<NormalUnitRigidBodyMovement>();

            normalUnitRigidBodyMovement.HP = 100.0f;
            normalUnitRigidBodyMovement.TargetString = Target;
            normalUnitRigidBodyMovement.AttackEnabled = AttackEnabled;
            normalUnitRigidBodyMovement.Damage = Damage;
            normalUnitRigidBodyMovement.Defense = Defense;

            normalUnitRigidBodyMovement.Spawner = this;
            normalUnitRigidBodyMovement.Initializing();
        }

        if (unitObj == null) return null;

        unitObj.GetComponent<NormalUnitRigidBodyMovement>()._Pool = this.Pool;

        return unitObj.gameObject;
    }

    // 사용 
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
    
}

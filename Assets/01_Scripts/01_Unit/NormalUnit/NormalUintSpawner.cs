using UnityEngine;
using Fusion;
using Agit.FortressCraft;
using UnityEngine.Pool;

public class NormalUintSpawner : NetworkBehaviour
{
    public NetworkPrefabRef UnitPrefab;
    public Player player = null;
    public bool Usable { get; private set; }

    [SerializeField] private string initialTarget = "";

    public IObjectPool<NetworkObject> Pool { get; set; }
    [SerializeField] private int defaultCapacity = 5;
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

        //GetComponent<NetworkObject>().RequestStateAuthority();

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

        if (player != null)
        {
            InitPooling();
        }
    }

    public void SpawnUnit()
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
        if (Runner.IsSharedModeMasterClient)
        {
            Pool.Get();
        }
    }

    // Object Pooling
    private void InitPooling()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        Pool = new ObjectPool<NetworkObject>(CreatPooledItem, OnTakeFromPool,
                                             OnReturnedToPool, OnDestroyPoolObject,
                                             true, defaultCapacity, maxPoolSize);

        /*
        // 미리 생성
        for (int i = 0; i < defaultCapacity; ++i)
        {
            GameObject no = CreatPooledItem();
            NormalUnitRigidBodyMovement normal = no.GetComponent<NormalUnitRigidBodyMovement>();
            normal._Pool.Release(normal.gameObject);
        }
        */
    }

    // 생성 
    private NetworkObject CreatPooledItem()
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

        return unitObj;
    }

    // 사용 
    private void OnTakeFromPool(NetworkObject poolNo)
    {
        //Debug.Log("TakeFromPool");
        poolNo.transform.position = transform.position;
        NormalUnitRigidBodyMovement normal = poolNo.gameObject.GetComponent<NormalUnitRigidBodyMovement>();
        normal.HP = 100.0f;
        Debug.Log("11" + " " + poolNo.gameObject.name);
        RPCSetActive(poolNo);
        Debug.Log("22" + " " + poolNo.gameObject.name);
    }

    // 반환
    private void OnReturnedToPool(NetworkObject poolNo)
    {
        RPCSetUnactive(poolNo);
    }

    private void OnDestroyPoolObject(NetworkObject poolNo)
    {
        Destroy(poolNo.gameObject);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPCSetUnactive(NetworkObject poolNo)
    {
        poolNo.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPCSetActive(NetworkObject poolNo)
    {
        Debug.Log(poolNo.gameObject.name + " is activated");
        poolNo.gameObject.SetActive(true);
        Debug.Log(poolNo.gameObject.activeSelf);
    }
}

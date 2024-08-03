using UnityEngine;
using Fusion;
using Agit.FortressCraft;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class NormalUnitSpawner : NetworkBehaviour
    {
        public NetworkObject UnitPrefab_A;
        public NetworkObject UnitPrefab_B;
        public NetworkObject UnitPrefab_C;
        public NetworkObject UnitPrefab_D;
        private NetworkObject UnitPrefab;
        public NetworkObject Arrow;
        public Player player = null;
        public bool Usable { get; private set; }
        public NetworkObjectPoolManager poolManager;
        public Transform Center { get; set; }
        [SerializeField] public string SpawnerType { get; set; }
        [SerializeField] private int maxUnitCount = 5;
        public int NowUnitCount { get; set; }

        // RPC property
        public string Target { get; set; }
        public bool AttackEnabled { get; set; }
        public float Damage { get; set; }
        public float Defense { get; set; }

        public NetworkPrefabId id;
        public NetworkPrefabId arrowId;

        private int idx = -1;

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCTargetChange(string t)
        {
            Debug.Log("target: " + t);
            Target = t;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCChangeAttackEnabled()
        {
            AttackEnabled = !AttackEnabled;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCSettingDamage(float newDamage)
        {
            Damage = newDamage;
            Debug.Log("Damage: " + newDamage);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCSettingDefense(float newDefense)
        {
            Defense = newDefense;
            Debug.Log("Defense: " + Damage);
        }

        public override void Spawned()
        {
            Usable = false;

            poolManager = NetworkObjectPoolManager.Instance;
            //AttackEnabled = true;
            Damage = 20.0f;
            Defense = 1.0f;
            NowUnitCount = 0;
            Center = GameObject.Find("Center").transform;


            if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
            {
                idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);
                Debug.Log("idx: " + idx);
                switch (idx)
                {
                    case 0:
                        SpawnerType = "A";
                        UnitPrefab = UnitPrefab_A;
                        break;
                    case 1:
                        SpawnerType = "B";
                        UnitPrefab = UnitPrefab_B;
                        break;
                    case 2:
                        SpawnerType = "C";
                        UnitPrefab = UnitPrefab_C;
                        break;
                    case 3:
                        SpawnerType = "D";
                        UnitPrefab = UnitPrefab_D;
                        break;
                }
            }

            //Debug.Log("Spawner Type: " + SpawnerType);

            ChangeTarget changeTarget = GameObject.FindObjectOfType<ChangeTarget>();
            if (changeTarget.OwnType == null)
            {
                changeTarget.OwnType = SpawnerType;
                string targetBtnName = "";
                switch (SpawnerType)
                {
                    case "A":
                        targetBtnName = "Button_2";
                        break;
                    case "B":
                        targetBtnName = "Button_3";
                        break;
                    case "C":
                        targetBtnName = "Button_4";
                        break;
                    case "D":
                        targetBtnName = "Button_1";
                        break;
                }
                changeTarget.UpdateTargetButtonColor(targetBtnName);
            }

            if (idx > -1)
            {
                Usable = true;

                if( changeTarget.Target == null )
                {
                    switch (idx)
                    {
                        case 0:
                            Target = "B";
                            break;
                        case 1:
                            Target = "C";
                            break;
                        case 2:
                            Target = "D";
                            break;
                        case 3:
                            Target = "A";
                            break;
                    }
                    changeTarget.Target = Target;
                }
                else
                {
                    Target = changeTarget.Target;
                }
                
            }

            AttackEnabled = changeTarget.IsAttackOn;

            NetworkObject temp = Runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
            id = temp.NetworkTypeId.AsPrefabId;
            Destroy(temp.gameObject);
            poolManager.AddPoolTable(id);

            temp = Runner.Spawn(Arrow, (Vector2)transform.position, Quaternion.identity);
            arrowId = temp.NetworkTypeId.AsPrefabId;
            Destroy(temp.gameObject);
            poolManager.AddPoolTable(arrowId);
        }

        public void SpawnUnit()
        {
            if (NowUnitCount >= maxUnitCount) return;
            if (Object.HasStateAuthority)
            {
                NetworkObject unitObj = null;

                NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
                var result = poolManager.AcquirePrefabInstance(Runner, context, out unitObj);

                if (result == NetworkObjectAcquireResult.Success)
                {
                    unitObj.transform.position = transform.position;

                    NormalUnitRigidBodyMovement normalUnitRigidBodyMovement = unitObj.GetComponent<NormalUnitRigidBodyMovement>();
                    NetworkMecanimAnimator animator = unitObj.GetComponent<NetworkMecanimAnimator>();
                    normalUnitRigidBodyMovement.TargetString = Target;
                    normalUnitRigidBodyMovement.AttackEnabled = AttackEnabled;
                    normalUnitRigidBodyMovement.Damage = Damage;
                    normalUnitRigidBodyMovement.Defense = Defense;
                    normalUnitRigidBodyMovement.Spawner = this;
                    normalUnitRigidBodyMovement.OwnType = SpawnerType;
                    normalUnitRigidBodyMovement.HP = 100.0f;
                    RPCUnitSetting(normalUnitRigidBodyMovement);
                    animator.Animator.Play("IdleState");
                    normalUnitRigidBodyMovement.Initializing();

                    float range = 0.4f;
                    Vector3 offset = new Vector3(Random.Range(-range, range),
                                                 Random.Range(-range, range),
                                                 0.0f);

                    normalUnitRigidBodyMovement.RPCSetPos(transform.position + offset);

                    normalUnitRigidBodyMovement.RPCSetActive();
                    ++NowUnitCount;
                }
                else
                {
                    Debug.LogError("Pooling Failed");
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCUnitSetting(NormalUnitRigidBodyMovement normal)
        {
            normal.NoReward = false;
        }
    }
}
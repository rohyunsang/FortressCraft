using Fusion;
using FusionHelpers;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class NormalUnitSpawner : NetworkBehaviour
    {
        public NetworkObject UnitPrefab_A;
        public NetworkObject UnitPrefab_B;
        public NetworkObject UnitPrefab_C;
        public NetworkObject UnitPrefab_D;
        private NetworkObject UnitPrefab;

        public NetworkObject UnitPrefab_A_Team;
        public NetworkObject UnitPrefab_B_Team;

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

        public Mode mode;

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

            if (!HasStateAuthority) return;

            poolManager = NetworkObjectPoolManager.Instance;
            //AttackEnabled = true;
            Damage = NormalUnitDataManager.Instance.Attack;
            Defense = NormalUnitDataManager.Instance.Defense;
            NowUnitCount = 0;
            Center = GameObject.Find("Center").transform;

            ChangeTarget changeTarget = GameObject.FindObjectOfType<ChangeTarget>();

            if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
            {
                idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);

                if (gameManager.mode == Mode.Survival)
                {
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
                else
                {
                    string team = gameManager.TryGetPlayerTeam(Runner.LocalPlayer);
                    if (team == "A")
                    {
                        SpawnerType = "A";
                        UnitPrefab = UnitPrefab_A;
                    }
                    else
                    {
                        SpawnerType = "B";
                        UnitPrefab = UnitPrefab_B;
                    }
                }

                if (changeTarget.OwnType == "")
                {
                    changeTarget.OwnType = SpawnerType;
                    string targetBtnName = "";
                    switch (SpawnerType)
                    {
                        case "A":
                            targetBtnName = "Button_1";
                            break;
                        case "B":
                            targetBtnName = "Button_2";
                            break;
                        case "C":
                            targetBtnName = "Button_3";
                            break;
                        case "D":
                            targetBtnName = "Button_4";
                            break;
                    }
                    changeTarget.UpdateTargetButtonColor(targetBtnName);
                }

                if (idx > -1)
                {
                    Usable = true;

                    if (changeTarget.Target == null)
                    {
                        switch (idx)
                        {
                            case 0:
                                Target = "A";
                                break;
                            case 1:
                                Target = "B";
                                break;
                            case 2:
                                Target = "C";
                                break;
                            case 3:
                                Target = "D";
                                break;
                        }
                        changeTarget.Target = Target;
                    }
                    else
                    {
                        Target = changeTarget.Target;
                    }
                }
            }

            AttackEnabled = changeTarget.IsAttackOn;

            NetworkObject temp = Runner.Spawn(UnitPrefab, (Vector2)transform.position, Quaternion.identity);
            id = temp.NetworkTypeId.AsPrefabId;
            Destroy(temp.gameObject);
            poolManager.AddPoolTable(id);

            temp = null;
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

                    NormalUnitRigidBodyMovement unit = unitObj.GetComponent<NormalUnitRigidBodyMovement>();
                    NetworkMecanimAnimator animator = unitObj.GetComponent<NetworkMecanimAnimator>();
                    unit.TargetString = Target;
                    unit.AttackEnabled = AttackEnabled;
                    unit.Damage = Damage;
                    unit.Defense = Defense;
                    unit.Spawner = this;
                    unit.OwnType = SpawnerType;
                    unit.HP = GoogleSheetManager.GetUnitData(1).HP;
                    RPCUnitSetting(unit);
                    animator.Animator.Play("IdleState");
                    unit.Initializing();

                    float range = 0.4f;
                    Vector3 offset = new Vector3(Random.Range(-range, range),
                                                 Random.Range(-range, range),
                                                 0.0f);

                    RPCSetPos(unit, transform.position + offset);

                    //unit.RPCSetActive();
                    RPCSetActive(unit);

                    ++NowUnitCount;
                    BattleBarUIManager.Instance.RPCPlusUnitCount(SpawnerType);
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

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetActive(NormalUnitRigidBodyMovement normal)
        {
            normal.gameObject.SetActive(true);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetPos(NormalUnitRigidBodyMovement normal, Vector3 pos)
        {
            normal.transform.position = pos;
        }
    }
}
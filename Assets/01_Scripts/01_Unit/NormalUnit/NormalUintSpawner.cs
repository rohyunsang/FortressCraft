using UnityEngine;
using Fusion;
using Agit.FortressCraft;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class NormalUintSpawner : NetworkBehaviour
    {
        public NetworkObject UnitPrefab;
        public NetworkObject Arrow;
        public Player player = null;
        public bool Usable { get; private set; }
        public NetworkObjectPoolManager poolManager;
        [SerializeField] private string initialTarget = "";
        public Transform Center { get; set; }
        private string spawnerType;

        // RPC property
        public string Target { get; set; }
        public bool AttackEnabled { get; set; }
        public float Damage { get; set; }
        public float Defense { get; set; }

        public NetworkPrefabId id;
        public NetworkPrefabId arrowId;

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
        public void RPCSettingDamage(float newDefense)
        {
            Damage = newDefense;
            Debug.Log("Damage: " + newDefense);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCSettingDefense(float newDamage)
        {
            Damage = newDamage;
            Debug.Log("Defense: " + Damage);
        }

        public override void Spawned()
        {
            Usable = false;

            poolManager = NetworkObjectPoolManager.Instance;
            Target = initialTarget;
            AttackEnabled = true;
            Damage = 20.0f;
            Defense = 1.0f;

            Center = GameObject.Find("Center").transform;

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
                    switch( p.PlayerIndex )
                    {
                        case 0:
                            spawnerType = "A";
                            break;
                        case 1:
                            spawnerType = "B";
                            break;
                        case 2:
                            spawnerType = "C";
                            break;
                        case 3:
                            spawnerType = "D";
                            break;
                    }
                }
            }

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

            if (Runner.IsSharedModeMasterClient)
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
                    normalUnitRigidBodyMovement.OwnType = spawnerType;
                    animator.Animator.Play("IdleState");
                    normalUnitRigidBodyMovement.Initializing();

                    Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f),
                                                 Random.Range(-1.0f, 1.0f) + 1.0f,
                                                 0.0f);

                    normalUnitRigidBodyMovement.RPCSetPos(transform.position + offset);

                    normalUnitRigidBodyMovement.RPCSetActive();
                }
                else
                {
                    Debug.LogError("Pooling Failed");
                }
            }
        }
    }
}
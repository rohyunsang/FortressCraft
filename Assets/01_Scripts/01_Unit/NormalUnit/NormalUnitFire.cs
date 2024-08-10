using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

namespace Agit.FortressCraft
{
    public class NormalUnitFire : MonoBehaviour
    {
        [SerializeField] private NetworkObject arrow;
        public Transform TargetTranform { get; set; }
        public string TargetUnit { get; set; }
        public string SecondTargetUnit { get; set; }
        private NormalUnitRigidBodyMovement normalUnit;
        public NetworkObjectPoolManager poolManager;
        
        public NetworkPrefabId id;

        private void Awake()
        {
            poolManager = NetworkObjectPoolManager.Instance;
            normalUnit = GetComponent<NormalUnitRigidBodyMovement>();
        }

        public void Fire(NetworkRunner runner)
        {
            if (normalUnit.Spawner == null) return;
            Debug.Log("Fire");
            id = normalUnit.Spawner.arrowId;
            poolManager.AddPoolTable(id);
            TargetUnit = normalUnit.TargetUnit;

            if (TargetUnit.CompareTo("Unit_" + normalUnit.OwnType) == 0)
            {
                TargetUnit = SecondTargetUnit;
            }

            NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
            NetworkObject networkObject;
            poolManager.AcquirePrefabInstance(runner, context, out networkObject);
            Debug.Log(networkObject);
            networkObject.transform.position = transform.position;

            Arrow arrow = networkObject.GetComponent<Arrow>();
            arrow.TargetTransform = TargetTranform;
            arrow.ID = id;
            arrow.Normal = normalUnit;
            RPCSetActive(arrow, transform.position);
            arrow.ReserveRelease();

            AttackCollider attackCollider = networkObject.GetComponentInChildren<AttackCollider>();
            attackCollider.TargetUnit = TargetUnit;
            attackCollider.OwnType = normalUnit.OwnType;
            attackCollider.Damage = normalUnit.Damage;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetActive(Arrow arrow, Vector3 pos)
        {
            arrow.transform.position = pos;
            arrow.gameObject.SetActive(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

namespace Agit.FortressCraft
{
    public class NormalUnitFire : MonoBehaviour, INetworkRunnerCallbacks
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
            arrow.RPCSetActive(transform.position);
            arrow.ReserveRelease();

            AttackCollider attackCollider = networkObject.GetComponentInChildren<AttackCollider>();
            attackCollider.TargetUnit = TargetUnit;
            attackCollider.Damage = normalUnit.Damage;
        }



        #region unused Callbacks
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            throw new NotImplementedException();
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            throw new NotImplementedException();
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            throw new NotImplementedException();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            throw new NotImplementedException();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            throw new NotImplementedException();
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            throw new NotImplementedException();
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            throw new NotImplementedException();
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
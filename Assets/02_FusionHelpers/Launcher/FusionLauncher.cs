using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Photon.Voice;
using Photon.Voice.Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Agit.FortressCraft;
using Photon.Realtime;
using System.Threading.Tasks;

namespace FusionHelpers
{
	public class FusionLauncher : MonoBehaviour, INetworkRunnerCallbacks
	{
		private Action<NetworkRunner, ConnectionStatus, string> _connectionCallback;
		[SerializeField]	private FusionSession _sessionPrefab;

		public string playerName = null;

		public enum ConnectionStatus
		{
			Disconnected,
			Connecting,
			Failed,
			Connected,
			Loading,
			Loaded
		}
        #region Lobby List 
        public static FusionLauncher ConnectToLobby(string playerName, FusionSession sessionPrefab, Action<NetworkRunner, ConnectionStatus, string> onConnect)
		{
            FusionLauncher launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

			launcher.playerName = playerName;

			launcher.InternalConnectToLobby(sessionPrefab, onConnect);

            return launcher;
		}

		private void InternalConnectToLobby(FusionSession sessionPrefab, Action<NetworkRunner, ConnectionStatus, string> onConnect)
		{
            DontDestroyOnLoad(gameObject);

			_sessionPrefab = sessionPrefab;
            _connectionCallback = onConnect;

            NetworkRunner runner = gameObject.AddComponent<NetworkRunner>();
            runner.name = name;
			runner.ProvideInput = true;
            
			runner.JoinSessionLobby(SessionLobby.Shared);
        }

		public static void ConnectToSession(string region, INetworkSceneManager sceneManager, string room)
		{
			FusionLauncher launcher = GameObject.Find("Launcher").GetComponent<FusionLauncher>();

            launcher.InternalConnectToSession(region, sceneManager, room);
        }

		private async void InternalConnectToSession(string region, INetworkSceneManager sceneManager, string room)
		{
			NetworkRunner runner = GetComponent<NetworkRunner>();

            while (runner.State == NetworkRunner.States.Shutdown)
            {
				Debug.Log(runner.State);
                await Task.Delay(100); // 100ms wait loop time 
            }

            // Voice 
            gameObject.AddComponent<FusionVoiceClient>();

            NetworkSceneInfo scene = new NetworkSceneInfo();
            scene.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

            // An empty region will use the best region.
            Fusion.Photon.Realtime.PhotonAppSettings.Global.AppSettings.FixedRegion = region;  // voice PhotonAppVoice Setting 

            SetConnectionStatus(runner, ConnectionStatus.Connecting, "");

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = room,
                ObjectProvider = gameObject.AddComponent<PooledNetworkObjectProvider>(),
                SceneManager = sceneManager,
                Scene = scene,
                PlayerCount = 4
                
            });
        }

        #endregion

        #region Create And Join
        public static FusionLauncher Launch(GameMode mode, string region, string room, string playerName  , FusionSession sessionPrefab,
			INetworkSceneManager sceneLoader,
			Action<NetworkRunner, ConnectionStatus, string> onConnect)
		{
			FusionLauncher launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();
			
			launcher.playerName = playerName;

			launcher.InternalLaunch(mode,region,room, sessionPrefab, sceneLoader, onConnect);
			return launcher;
		}
		
		private async void InternalLaunch(GameMode mode, string region, string room,
            FusionSession sessionPrefab,
			INetworkSceneManager sceneManager,
			Action<NetworkRunner, ConnectionStatus, string> onConnect)
			{
			_sessionPrefab = sessionPrefab;
			_connectionCallback = onConnect;

			DontDestroyOnLoad(gameObject);
			
			NetworkRunner runner = gameObject.AddComponent<NetworkRunner>();
			runner.name = name;
			runner.ProvideInput = mode != GameMode.Server;

			// Voice 
			gameObject.AddComponent<FusionVoiceClient>();

			NetworkSceneInfo scene = new NetworkSceneInfo();
			scene.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

            // An empty region will use the best region.
            Fusion.Photon.Realtime.PhotonAppSettings.Global.AppSettings.FixedRegion = region;  // 

			SetConnectionStatus(runner, ConnectionStatus.Connecting, "");

			await runner.StartGame(new StartGameArgs()
			{
				GameMode = mode, 
				SessionName = room,
				ObjectProvider = gameObject.AddComponent<PooledNetworkObjectProvider>(),
				SceneManager = sceneManager,
				Scene = scene,
				PlayerCount = 4 // Max Player is 4 
			});
		}
        #endregion

        public void ShutDownCustom()
        {
            Invoke("ShutdownRunner", 2.0f);
        }

        private void ShutdownRunner()
        {
            NetworkRunner runner = GetComponent<NetworkRunner>();
            if (runner != null)
            {
                runner.Shutdown();
            }
            else
            {
                Debug.LogError("NetworkRunner component not found on the GameObject.");
            }
        }

        public void SetConnectionStatus(NetworkRunner runner, ConnectionStatus status, string message)
		{
			if (_connectionCallback != null)
				_connectionCallback(runner, status, message);
		}

		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("Connected to server");
			SetConnectionStatus(runner, ConnectionStatus.Connected, "");
		}

		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			Debug.Log("Disconnected from server");
			SetConnectionStatus(runner, ConnectionStatus.Disconnected, "");
		}

		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			request.Accept();
		}

		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			Debug.Log($"Connect failed {reason}");
			SetConnectionStatus(runner, ConnectionStatus.Failed, reason.ToString());
		}

		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log($"Player {player} Joined");
			if (runner.IsServer || runner.IsSharedModeMasterClient) {
				if(!runner.TryGetSingleton(out FusionSession session) && _sessionPrefab != null)
				{
					Debug.Log($"I am {(runner.IsServer ? "Server":"Master")} and I do not have a session - Spawning Session");
					session = runner.Spawn(_sessionPrefab);
				}
				session.PlayerJoined(player);
			}
		}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if(runner.TryGetSingleton(out FusionSession session))
				session.PlayerLeft(player);
		}

		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			Debug.Log("OnShutdown");
			string message = "";
			switch (shutdownReason)
			{
				case ShutdownReason.IncompatibleConfiguration:
					message = "This room already exist in a different game mode!";
					break;
				case ShutdownReason.Ok:
					message = "User terminated network session!"; 
					break;
				case ShutdownReason.Error:
					message = "Unknown network error!";
					break;
				case ShutdownReason.ServerInRoom:
					message = "There is already a server/host in this room";
					break;
				case ShutdownReason.DisconnectedByPluginLogic:
					message = "The Photon server plugin terminated the network session!";
					break;
				default:
					message = shutdownReason.ToString();
					break;
			}

			SetConnectionStatus(runner, ConnectionStatus.Disconnected, message);
			runner.ClearRunnerSingletons();
			Destroy(gameObject);
		}

		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
            Debug.Log($"Session List Updated with {sessionList.Count} session(s)");
            // RoomListPanel - Content - Destroy

            RoomListPanel roomListPanel = FindObjectOfType<RoomListPanel>();
            if (roomListPanel != null && roomListPanel._scrollViewContent != null)
            {
                foreach (Transform child in roomListPanel._scrollViewContent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                Debug.Log("RoomListPanel or _scrollViewContent is not found!");
            }

            if (sessionList.Count > 0)
            {
                // Prefab Spawning 
                foreach (var session in sessionList)
				{
                    Debug.Log($"SessionName {session.Name}");
                    Debug.Log($"SessionPlayerCount {session.PlayerCount}");
                    Debug.Log($"SessionPlayerCount {session.MaxPlayers}");

                    GameObject sessionEntryPrefab =  Instantiate(FindObjectOfType<RoomListPanel>()._sessionEntryPrefab, FindObjectOfType<RoomListPanel>()._scrollViewContent);
					sessionEntryPrefab.GetComponent<SessionEntryPrefab>().init(session.Name, session.PlayerCount.ToString(), session.MaxPlayers.ToString());
                }
            }
        }


		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
		public void OnSceneLoadStart(NetworkRunner runner) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnInput(NetworkRunner runner, NetworkInput input) { }
		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	}
}
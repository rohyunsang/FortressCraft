using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Agit.FortressCraft;

namespace FusionHelpers
{
	/// <summary>
	/// Base class for you per-session state class.
	/// You can use this to track and access player avatars on all peers.
	/// Override OnPlayerAvatarAdded/Removed to be notified of players joining/leaving *after* their avatar is created or removed.
	/// Use GetPlayer/GetPlayerByIndex/AllPlayers to access or iterate over players on all peers.
	/// Use Runner.GetSingleton/Runner.WaitForSingleton to get your custom session instance on all peers.
	/// </summary>

	public abstract class FusionSession : NetworkBehaviour
	{
		private const int MAX_PLAYERS = 4;
		
		[SerializeField] private FusionPlayer _playerPrefab;
        [SerializeField] private FusionPlayer _playerPrefabWarrior;
        [SerializeField] private FusionPlayer _playerPrefabArcher;
		[SerializeField] private FusionPlayer _playerPrefabMagician;
		[SerializeField] private FusionPlayer _RPG_Player;

        [Networked, Capacity(10)] public NetworkDictionary<int, PlayerRef> playerRefByIndex { get; }
		private Dictionary<PlayerRef, FusionPlayer> _players = new();

		protected abstract void OnPlayerAvatarAdded(FusionPlayer fusionPlayer);
		protected abstract void OnPlayerAvatarRemoved(FusionPlayer fusionPlayer);

		public IEnumerable<FusionPlayer> AllPlayers => _players.Values;
		public int PlayerCount => _players.Count;
		public int SessionCount => playerRefByIndex.Count;

		private bool isSetCommanderType = false;
		
		public override void Spawned()
		{
			isSetCommanderType = false;

            Debug.Log($"Spawned Network Session for Runner: {Runner}");
            Runner.RegisterSingleton(this);

                // App에서 플레이어 직업 타입을 정한다.
                JobType jobType = FindObjectOfType<App>().jobType;
                switch (jobType)
                {
                    case JobType.Warrior:
                        _playerPrefab = _playerPrefabWarrior;
                        break;
                    case JobType.Archer:
                        _playerPrefab = _playerPrefabArcher;
                        break;
                    case JobType.Magician:
                        _playerPrefab = _playerPrefabMagician;
                        break;
                    default:
                        _playerPrefab = _playerPrefabArcher; // default commander is Archer
                        Debug.LogError("default commander is Archer");
                        break;
                }
			

            
			isSetCommanderType = true;
        }

		public override void Render()
		{
			if (!isSetCommanderType) return;
            
			 if (Runner && Runner.Topology == Topologies.Shared && _players.Count != playerRefByIndex.Count)
				MaybeSpawnNextAvatar();
        }

        private void MaybeSpawnNextAvatar()
		{
			Debug.Log("Maybe");
			foreach (KeyValuePair<int,PlayerRef> refByIndex in playerRefByIndex)
			{
				if (Runner.IsServer || (Runner.Topology == Topologies.Shared && refByIndex.Value == Runner.LocalPlayer))
				{
					if (!_players.TryGetValue(refByIndex.Value, out _))
					{
						// Debug.Log($"I am State Auth for Player Index {refByIndex.Key} - Spawning Avatar");
						Runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, refByIndex.Value, (runner, o) =>
						{
							Runner.SetPlayerObject(refByIndex.Value, o);
							FusionPlayer player = o.GetComponent<FusionPlayer>();
							if (player != null)
							{
								player.NetworkedPlayerIndex = refByIndex.Key;
								player.InitNetworkState();
							}
						});
					}
				}
			}
		}

		public void AddPlayerAvatar(FusionPlayer fusionPlayer)
		{
			Debug.Log($"Adding PlayerRef {fusionPlayer.PlayerId}");
			_players[fusionPlayer.PlayerId] = fusionPlayer;
			OnPlayerAvatarAdded(fusionPlayer);
		}

		public void RemovePlayerAvatar(FusionPlayer fusionPlayer)
		{
			Debug.Log($"Removing PlayerRef {fusionPlayer.PlayerId}");
			_players.Remove(fusionPlayer.PlayerId);
			if(Object != null && Object.IsValid)
				playerRefByIndex.Remove(fusionPlayer.PlayerIndex);
			OnPlayerAvatarRemoved(fusionPlayer);
		}

		public T GetPlayer<T>(PlayerRef plyRef) where T: FusionPlayer
		{
			_players.TryGetValue(plyRef, out FusionPlayer ply);
			return (T)ply;
		}

		public T GetPlayerByIndex<T>(int idx) where T: FusionPlayer
		{
			foreach (FusionPlayer player in _players.Values)
			{
				if (player.Object!=null && player.Object.IsValid && player.PlayerIndex == idx)
					return (T)player;
			}
			return default;
		}

		private int NextPlayerIndex()
		{
			for (int idx=0;idx<Runner.Config.Simulation.PlayerCount;idx++)
			{
				if (!playerRefByIndex.TryGet(idx, out _) )
					return idx;
			}
			Debug.LogWarning("No free player index!");
			return -1;
		}

		public void PlayerLeft(PlayerRef playerRef)
		{
			Debug.Log($"Player {playerRef} Left");

			if (!Runner.IsShutdown)
			{
				FusionPlayer player = GetPlayer<FusionPlayer>(playerRef);
				if (player && player.Object.IsValid)
				{
					Debug.Log($"Despawning PlayerAvatar for PlayerRef {player.PlayerId}");
					Runner.Despawn(player.Object);
				}
				
				// This means only on player remains
				if (Runner.SessionInfo.PlayerCount >= 1)
                {

                }
				else
				{
                    Runner.Shutdown(false);
                    Debug.Log("AAAAAAA" + Runner.SessionInfo.PlayerCount);
				}
			}
		}

		public void PlayerJoined(PlayerRef player)
		{
			int nextIndex = NextPlayerIndex();

			Debug.Log($"I am {Runner.LocalPlayer} and I am {(Runner.IsServer ? "Server":"Master")}. The Session StateAuth is: {Object.StateAuthority} - Assigning Index {nextIndex} to PlayerRef {player}");
			playerRefByIndex.Set(nextIndex, player);
			MaybeSpawnNextAvatar();
		}
	}
}
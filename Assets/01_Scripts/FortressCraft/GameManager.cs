using UnityEngine;
using Fusion;
using FusionHelpers;
using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

namespace Agit.FortressCraft
{
	public class GameManager : FusionSession
	{
		public enum PlayState { LOBBY, LEVEL, TRANSITION }

		[Networked] public PlayState currentPlayState { get; set; }
		[Networked, Capacity(4)] private NetworkArray<int> score => default;

		[Networked, Capacity(32)] public NetworkDictionary<string, int> playerRef_playerIdx => default;

		public Player lastPlayerStanding { get; set; }
		public Player matchWinner
		{
			get
			{
				for (int i = 0; i < score.Length; i++)
				{
					if (score[i] >= MAX_SCORE)
						return GetPlayerByIndex<Player>(i);
				}
				return null;
			}
		}

		public const byte MAX_SCORE = 3;

		private bool _restart;

		public bool DisconnectByPrompt { get; set; }

		public override void Spawned()
		{
			base.Spawned();

			Runner.RegisterSingleton(this);

			if (Object.HasStateAuthority)
			{
				LoadLevel(-1);

            }
			else if (currentPlayState != PlayState.LOBBY)
			{
				Debug.Log("Rejecting Player, game is already running!");
				_restart = true;
			}

			FindObjectOfType<UIManager>().startButton.onClick.AddListener(GameStartButtonCallback);
			FindObjectOfType<UIManager>().leaveToSessionButton.onClick.AddListener(DisconnectSession);
            FindObjectOfType<UIManager>().leaveToGameButtonVictoryPanel.onClick.AddListener(DisconnectSession);
            FindObjectOfType<UIManager>().leaveToGameButtonDefeatPanel.onClick.AddListener(DisconnectSession);
            
        }
        
        public void GetDestroyCastlePlayerRef(string team)
		{
            foreach (FusionPlayer fusionPlayer in AllPlayers)
            {
                Player player = (Player)fusionPlayer;
				if (player.PlayerId.ToString().Contains(team))
				{
					player.RPC_SetDestroyCastle(player.PlayerId);
                    break;
				}
            }
        }

		public void GetWinnerPlayerRef(string team)
		{
            foreach (FusionPlayer fusionPlayer in AllPlayers)
            {
                Player player = (Player)fusionPlayer;
                if (player.PlayerId.ToString().Contains(team))
                {
                    player.RPC_Winner(player.PlayerId);
                    break;
                }
            }
        }

		private void MakeDictionaryPlayerIdUsingPlayerRef()
		{
            foreach (FusionPlayer fusionPlayer in AllPlayers)
            {
                Player player = (Player)fusionPlayer;
                playerRef_playerIdx.Add(player.PlayerId.ToString() , player.PlayerId.PlayerId);

                Debug.Log(player.PlayerId);
				Debug.Log(player.PlayerId.PlayerId);
            }
            var sortedDict = playerRef_playerIdx.OrderBy(pair => pair.Value).ToList();

            int i = 0;

            foreach (var pair in sortedDict)
            {
                Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
				playerRef_playerIdx.Set(pair.Key ,i);
				i++;
            }

            foreach (KeyValuePair<string, int> entry in playerRef_playerIdx)
            {
                Debug.Log($"playerRef: {entry.Key}, PlayerId: {entry.Value}");
            }
        }

		public int TryGetPlayerId(PlayerRef playerRef)
		{
			return playerRef_playerIdx[playerRef.ToString()];
        }

        protected override void OnPlayerAvatarAdded(FusionPlayer fusionPlayer)
		{
			// Runner.GetLevelManager()?.cameraStrategy.AddTarget(((Player)fusionPlayer).cameraTarget);
		}

		protected override void OnPlayerAvatarRemoved(FusionPlayer fusionPlayer)
		{
			// Runner.GetLevelManager()?.cameraStrategy.RemoveTarget(((Player)fusionPlayer).cameraTarget);
		}
        public void OnCommanderDeath()
		{
			if (currentPlayState != PlayState.LOBBY)
			{
				int playersLeft = 0;
				lastPlayerStanding = null;

				foreach (FusionPlayer fusionPlayer in AllPlayers)
				{
					Player player = (Player)fusionPlayer;
					if (player.isActivated || player.lives > 0)
					{
						lastPlayerStanding = player;
						playersLeft++;
					}
				}

				if (playersLeft > 1)
					lastPlayerStanding = null;

				Debug.Log($"Someone died - {playersLeft} left");
				if (lastPlayerStanding != null)
				{
					int nextLevelIndex = Runner.GetLevelManager().GetBattleSceneIndex();
					int newScore = score[lastPlayerStanding.PlayerIndex] + 1;
					if (HasStateAuthority)
						score.Set(lastPlayerStanding.PlayerIndex, newScore);
					if (newScore >= MAX_SCORE)
						nextLevelIndex = -1;
					LoadLevel(nextLevelIndex);
				}
			}
		}

		public void Restart(ShutdownReason shutdownReason)
		{
			if (!Runner.IsShutdown)
			{
				// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
				Runner.Shutdown(false, shutdownReason);
				_restart = false;
			}
		}

		public const ShutdownReason ShutdownReason_GameAlreadyRunning = (ShutdownReason)100;

		private void Update()
		{
			if (_restart || DisconnectByPrompt)
			{
				Restart(_restart ? ShutdownReason_GameAlreadyRunning : ShutdownReason.Ok);
				_restart = false;

				DisconnectByPrompt = true;
			}
		}

		public void DisconnectSession() // using Button 
		{
            var DisconnectManager = FindObjectOfType<DisconnectManager>();
            if (DisconnectManager && !DisconnectManager.DisconnectPrompt.activeSelf)
                DisconnectManager.DisconnectPrompt.SetActive(true);
        }

		private void ResetStats()
		{
			if (!HasStateAuthority)
				return;
			for (int i = 0; i < score.Length; i++)
				score.Set(i, 0);
		}

		// Transition from lobby to level
		public void GameStartButtonCallback()
		{
			Debug.Log("Lets go Start");

			if(Object.HasStateAuthority)
				MakeDictionaryPlayerIdUsingPlayerRef(); // only bangjang

            // close and hide the session from matchmaking / lists. this demo does not allow late join.
            Runner.SessionInfo.IsOpen = false;
			Runner.SessionInfo.IsVisible = false;

			// Reset stats and transition to level.
			ResetStats();
			LoadLevel(Runner.GetLevelManager().GetBattleSceneIndex());
		}

		private void LoadLevel(int nextLevelIndex)
		{
			if (Object.HasStateAuthority)
				Runner.GetLevelManager().LoadLevel(nextLevelIndex);
		}

		public int GetScore(Player player)
		{
			return score[player.PlayerIndex];
		}
	}
}
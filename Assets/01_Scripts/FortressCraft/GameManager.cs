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


		[Networked, Capacity(32)] public NetworkDictionary<string, int> playerRef_playerIdx => default;


		public const byte MAX_SCORE = 3;

		private bool _restart;

		public bool DisconnectByPrompt { get; set; }

		public bool isStarted = false;

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
        
        public void GetDestroyCastleOwnerPlayer(string tag)
		{
            foreach (FusionPlayer fusionPlayer in AllPlayers)
            {
                Player player = (Player)fusionPlayer;
                // "UnitRoot"라는 이름의 자식 오브젝트를 찾습니다.
                Transform unitRoot = player.transform.Find("UnitRoot");
                if (unitRoot != null && unitRoot.gameObject.tag == tag)
                {
					player.RPC_CastleCountDown(player.PlayerId);
					if(player.castleCount <= 0)
					{
						player.RPC_DestroyedAllCastle(player.PlayerId);
						FindWinner();
                    }
                    break;
                }
            }
        }

        public void FindWinner()
		{
			int allPlayerCount = 0;
			int playerDefeatCnt = 0;
            foreach (FusionPlayer fusionPlayer in AllPlayers)
            {
                allPlayerCount++;
            }
			foreach(FusionPlayer fusionPlayer in AllPlayers)
			{
                Player player = (Player)fusionPlayer;
				if(player.IsDestroyedAllCastle)
					playerDefeatCnt++;
            }

			if (allPlayerCount - 1 == playerDefeatCnt)
			{
				foreach (FusionPlayer fusionPlayer in AllPlayers)
				{
                    Player player = (Player)fusionPlayer;
					if (!player.IsDestroyedAllCastle)
					{
						player.RPC_Winner(player.PlayerId);	
                    }
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
		}

		protected override void OnPlayerAvatarRemoved(FusionPlayer fusionPlayer)
		{
		}
		public void Restart(ShutdownReason shutdownReason)
		{
			if (!Runner.IsShutdown)
			{
				// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
				Runner.Shutdown(false, shutdownReason);
				_restart = false;

				GameObject recorder = GameObject.Find("[Recorder]");
                GameObject recorderLogger = GameObject.Find("VoiceLogger");
				if(recorder != null)
				{
					Destroy(recorder);
				}
				if(recorderLogger != null)
				{
					Destroy(recorderLogger);
				}
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

		// Transition from lobby to level
		public void GameStartButtonCallback()
		{
			if (isStarted) return;
			if (!Object.HasStateAuthority) return;

			isStarted = true;

			MakeDictionaryPlayerIdUsingPlayerRef(); // only bangjang

			// close and hide the session from matchmaking / lists. this demo does not allow late join.
			Runner.SessionInfo.IsOpen = false;
			Runner.SessionInfo.IsVisible = false;

			// Reset stats and transition to level.
			Invoke("InvokeLoadLevel",2f);
		}

		private void InvokeLoadLevel()
		{
            int nextLevelIndex = Runner.GetLevelManager().GetBattleSceneIndex();
            if (Object.HasStateAuthority)
                Runner.GetLevelManager().LoadLevel(nextLevelIndex);
        }

		private void LoadLevel(int nextLevelIndex)
		{
            if (Object.HasStateAuthority)
				Runner.GetLevelManager().LoadLevel(nextLevelIndex);
		}
	}
}
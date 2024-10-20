using Fusion;
using FusionHelpers;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public enum SceneIndex
    {
        Main = 0,
        Lobby = 1,
        Battle = 2
    }


    /// <summary>
    /// The LevelManager controls the map - keeps track of spawn points for players.
    /// </summary>
    /// TODO: This is partially left over from previous SDK versions which had a less capable SceneManager, so could probably be simplified quite a bit
    public class LevelManager : NetworkSceneManagerDefault
    {
        // [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private CountdownManager _countdownManager;

        [SerializeField] private int _lobby;
        [SerializeField] private int[] _levels;

        private LevelBehaviour _currentLevel;
        private SceneRef _loadedScene = SceneRef.None;

        public Action<NetworkRunner, FusionLauncher.ConnectionStatus, string> onStatusUpdate { get; set; }

        [SerializeField] public Text roomCode;

        public SpawnPoint GetPlayerSpawnPoint(int playerIndex)
        {
            if (_currentLevel != null)
                return _currentLevel.GetPlayerSpawnPoint(playerIndex);
            return null;
        }

        public override void Shutdown()
        {
            Debug.Log("LevelManager.Shutdown();");
            _currentLevel = null;
            if (_loadedScene.IsValid)
            {
                Debug.Log($"LevelManager.UnloadLevel(); - _currentLevel={_currentLevel} _loadedScene={_loadedScene}");
                SceneManager.UnloadSceneAsync(_loadedScene.AsIndex);
                _loadedScene = SceneRef.None;
            }
            // _scoreManager.ResetAllGameScores();
            base.Shutdown();
        }

        public int GetBattleSceneIndex()
        {
            int idx = (int)SceneIndex.Battle;
            return idx;
        }

        public void LoadLevel(int nextLevelIndex)
        {
            _currentLevel = null;
            if (_loadedScene.IsValid)
            {
                Debug.Log($"LevelManager.UnloadLevel(); - _currentLevel={_currentLevel} _loadedScene={_loadedScene}");
                Runner.UnloadScene(_loadedScene);
                _loadedScene = SceneRef.None;
            }
            // Debug.Log($"LevelManager.LoadLevel({nextLevelIndex});");
            if (nextLevelIndex < 0)
            {
                Runner.LoadScene(SceneRef.FromIndex(_lobby), new LoadSceneParameters(LoadSceneMode.Additive), true);
            }
            else
            {
                Runner.LoadScene(SceneRef.FromIndex((int)SceneIndex.Battle), new LoadSceneParameters(LoadSceneMode.Additive), true);
            }
        }

        protected override IEnumerator UnloadSceneCoroutine(SceneRef prevScene)
        {
            Debug.Log($"LevelManager.UnloadSceneCoroutine({prevScene});");


            GameManager gameManager;
            while (!Runner.TryGetSingleton(out gameManager))
            {
                Debug.LogWarning("Waiting for GameManager");
                yield return null;
            }

            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                gameManager.currentPlayState = GameManager.PlayState.TRANSITION;

            if (prevScene.AsIndex > 0)
            {
                InputController.fetchInput = false;
            }

            yield return base.UnloadSceneCoroutine(prevScene);
        }

        protected override IEnumerator OnSceneLoaded(SceneRef newScene, Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
        {
            Debug.Log($"LevelManager.OnSceneLoaded({newScene},{loadedScene},{sceneFlags});");

            yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

            if (newScene.AsIndex == 0)
                yield break;


            onStatusUpdate?.Invoke(Runner, FusionLauncher.ConnectionStatus.Loading, "");

            yield return null;

            _loadedScene = newScene;
            Debug.Log($"Loading scene {newScene}");

            // Delay one frame
            yield return null;

            onStatusUpdate?.Invoke(Runner, FusionLauncher.ConnectionStatus.Loaded, "");

            // Activate the next level
            _currentLevel = FindObjectOfType<LevelBehaviour>();

            yield return new WaitForSeconds(1.0f);

            GameManager gameManager;
            while (!Runner.TryGetSingleton(out gameManager))
            {
                Debug.Log($"Waiting for GameManager to Spawn!");
                yield return null;
            }

            // Respawn with slight delay between each player
            Debug.Log($"Respawning All {gameManager.PlayerCount} Players");
            var players = gameManager.AllPlayers;
            foreach (FusionPlayer fusionPlayer in players)
            {
                Player player = (Player)fusionPlayer;
                Debug.Log($"Initiating Respawn of Player #{fusionPlayer.PlayerIndex} ID:{fusionPlayer.PlayerId}:{player}");
                player.Reset();
                player.Respawn();
                
            }

            // Set state to playing level
            if (_loadedScene.AsIndex == _lobby)
            {
                if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                    gameManager.currentPlayState = GameManager.PlayState.LOBBY;
                InputController.fetchInput = true;
            }
            else
            {
                StartCoroutine(_countdownManager.Countdown(() =>
                {
                    // Set state to playing level
                    if (Runner != null && (Runner.IsServer || Runner.IsSharedModeMasterClient))
                    {
                        gameManager.currentPlayState = GameManager.PlayState.LEVEL;
                    }
                    // Enable inputs after countdow finishes
                    InputController.fetchInput = true;
                }));
            }
        }
    }
}
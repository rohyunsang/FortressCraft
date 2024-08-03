using UnityEngine;

namespace Agit.FortressCraft
{
	public class LevelBehaviour : MonoBehaviour
	{
		// Class for storing the lighting settings of a level
		[System.Serializable]
		public struct LevelLighting
		{
			public Color ambientColor;
			public Color fogColor;
			public bool fog;
		}

		[SerializeField] private LevelLighting _levelLighting;

		public SpawnPoint[] _playerSpawnPoints;

		public string currentSceneName = "";
		
		public SpawnPoint GetPlayerSpawnPoint(int plyIndex)
		{
			return _playerSpawnPoints[plyIndex].GetComponent<SpawnPoint>();
		}

        public void Start()  // UI Changer Call
		{
			currentSceneName = gameObject.name;

			if (gameObject.name == "Battle")
			{
				FindObjectOfType<LevelUIController>().BattleSceneUIChange();

				Invoke("ChangeCommanderJob", 2f);
				Invoke("SpawnCastle", 3f);
            }
        }
		private void SpawnCastle()
		{
            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                player._spawnCastle.SpawnCastleObject();
            }
        }

		private void ChangeCommanderJob()
		{
            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                player.RPC_CommanderJobChanger();
            }
        }
    }
}
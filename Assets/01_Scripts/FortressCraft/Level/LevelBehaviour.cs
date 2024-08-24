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

		[SerializeField] private GameObject BGM_Lobby;
		[SerializeField] private GameObject BGM_Battle;

        public SpawnPoint GetPlayerSpawnPoint(int plyIndex)
		{
			return _playerSpawnPoints[plyIndex].GetComponent<SpawnPoint>();
		}

        public void Start()  // UI Changer Call
		{
			currentSceneName = gameObject.name;

			if(gameObject.name == "Lobby")
			{
				Instantiate(BGM_Lobby).gameObject.name = "BGM";
			}

			if (gameObject.name == "Battle")
			{
				FindObjectOfType<UIManager>().LoadingMsg.SetActive(false);


                GameObject bgm = FindObjectOfType<BGM>().gameObject;
                if (bgm != null)
                    Destroy(FindObjectOfType<BGM>().gameObject);

                Instantiate(BGM_Battle).gameObject.name = "BGM";

                FindObjectOfType<LevelUIController>().BattleSceneUIChange();

				Invoke("SpawnCastle", 2f);
            }
            if (gameObject.name == "RPG")
			{
                FindObjectOfType<LevelUIController>().RPGSceneUIChange();
            }
        }

		private void SpawnCastle() // using Invoke 
		{
            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                player._spawnCastle.SpawnCastleObject();
            }
        }
    }
}
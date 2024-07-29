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
		
		public SpawnPoint GetPlayerSpawnPoint(int plyIndex)
		{
			return _playerSpawnPoints[plyIndex].GetComponent<SpawnPoint>();
		}

        public void Start()  // UI Changer Call
		{
			Debug.Log("AAAAAAAA" + gameObject.name);

			if (gameObject.name == "Battle")
			{
				FindObjectOfType<LevelUIController>().BattleSceneUIChange();
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Agit.FortressCraft
{
	/// <summary>
	/// SpawnPoint is used by the LevelBehaviour to figure out where it can spawn things.
	/// There is nothing Fusion specific about this.
	/// </summary>
	public class SpawnPoint : MonoBehaviour
	{
		#if UNITY_EDITOR
		private static bool _spawnPointIsSelected;

		private void OnDrawGizmosSelected()
		{
			// If the selected object contains a spawn point, show gizmos for ALL spawn points.
			CheckIfSpawnPointIsSelected();
		}

		private void OnDrawGizmos()
		{
			// If one spawn point is selected, all spawn points will draw a gizmo.
			if (_spawnPointIsSelected)
			{
				// Check if spawn point has since been deselected.
				if (CheckIfSpawnPointIsSelected() == false)
				{
					return;
				}

				const float FORWD_LEN = 2;
				const float OTHER_LEN = 1f;

				var pos = transform.position;
				var forward = transform.forward;
				var right = transform.right;
				var up = transform.up;

				//Gizmos.color = Color.yellow;
				//Gizmos.DrawWireSphere(pos, 1);
				Gizmos.DrawRay(pos, forward * FORWD_LEN);
				Gizmos.DrawRay(pos, right * OTHER_LEN);
				Gizmos.DrawRay(pos, up * OTHER_LEN);
				Gizmos.color = Color.white;
				Gizmos.DrawSphere(pos, .5f);

				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(pos + forward * FORWD_LEN, .25f);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(pos + right * OTHER_LEN, .25f);
				Gizmos.DrawSphere(pos - right * OTHER_LEN, .25f);
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(pos + up * OTHER_LEN, .25f);
			}
		}

		private bool CheckIfSpawnPointIsSelected()
		{
			if (Selection.activeGameObject != null)
				_spawnPointIsSelected = Selection.activeGameObject.GetComponent<SpawnPoint>();
			else
				_spawnPointIsSelected = false;

			return _spawnPointIsSelected;
		}
#endif
	}
}
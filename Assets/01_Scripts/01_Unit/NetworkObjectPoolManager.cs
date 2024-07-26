using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
	public class NetworkObjectPoolManager : NetworkObjectProviderDefault
	{
		public static NetworkObjectPoolManager Instance { get; set; }

		private Dictionary<NetworkObjectTypeId, Stack<NetworkObject>> _poolTable = new();

		private void Awake()
		{
			Instance = this;
		}

		public void AddPoolTable(NetworkObjectTypeId id)
		{
			if (!_poolTable.ContainsKey(id))
			{
				_poolTable.Add(id, new Stack<NetworkObject>());
			}
		}

		protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
		{
			NetworkObject instance = null;

			instance = GetFromObjetPool(runner, prefab);


			if (instance == null)
			{
				instance = runner.Spawn(prefab, (Vector2)transform.position, Quaternion.identity);
			}

			return instance;
		}

		private NetworkObject GetFromObjetPool(NetworkRunner runner, NetworkObject prefab)
		{
			NetworkObject instance = null;

			if (_poolTable.TryGetValue(prefab.NetworkTypeId, out var pool) == true)
			{
				if (pool.TryPop(out var pooledObject) == true)
				{
					instance = pooledObject;
					//Debug.Log("Pop! - " + instance.name);
				}
			}

			if (instance == null)
			{
				instance = GetNewInstance(runner, prefab);
			}

			return instance;
		}

		private NetworkObject GetNewInstance(NetworkRunner runner, NetworkObject prefab)
		{
			NetworkObject instance = runner.Spawn(prefab, (Vector2)transform.position, Quaternion.identity);

			if (_poolTable.TryGetValue(prefab.NetworkTypeId, out var stack) == false)
			{
				stack = new Stack<NetworkObject>();
				_poolTable.Add(prefab.NetworkTypeId, stack);
			}

			return instance;
		}

		protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
		{
			if (_poolTable.TryGetValue(prefabId, out var stack) == true)
			{
				Debug.Log("DestroyPrefabInstance - Pooling");
				instance.transform.SetParent(null);
				instance.gameObject.SetActive(false);
				if (instance.TryGetComponent<NormalUnitRigidBodyMovement>(out NormalUnitRigidBodyMovement normal))
				{
					normal.RPCSetUnactive();
				}
				else if (instance.TryGetComponent<Arrow>(out Arrow arrow))
                {
					Debug.Log("Arrow unactive");
					arrow.RPCSetUnactive();
                }
				stack.Push(instance);
			}
			else
			{
				//Debug.Log("DestroyPrefabInstance - Destroy");
				Destroy(instance.gameObject);
			}
		}
	}
}
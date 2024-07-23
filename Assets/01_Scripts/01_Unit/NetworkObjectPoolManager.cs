using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkObjectPoolManager : NetworkObjectProviderDefault
{
	[SerializeField] private List<NetworkObject> _poolableObjects;
	private Dictionary<NetworkObjectTypeId, Stack<NetworkObject>> _poolTable = new();

	public void AddPoolTable(NetworkObjectTypeId id)
    {
		if( !_poolTable.ContainsKey(id) )
        {
			_poolTable.Add(id, new Stack<NetworkObject>() );
        }
    }

    protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
	{
		NetworkObject instance = null;


		if (ShouldPool(prefab) == true)
		{
			instance = GetFromObjetPool(runner, prefab);
			instance.transform.position = Vector3.zero;
		}

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
			}
		}

		if (instance == null)
		{
			instance = GetNewInstance(runner, prefab);
		}

		instance.gameObject.SetActive(true);
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

	private bool ShouldPool(NetworkObject prefab)
	{
		foreach (var poolableObjct in _poolableObjects)
		{
			if (prefab == poolableObjct)
				return true;
		}
		return false;
	}

	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		if (_poolTable.TryGetValue(prefabId, out var stack) == true)
		{
			Debug.Log("DestroyPrefabInstance - Pooling");
			instance.transform.SetParent(null);
			//instance.gameObject.SetActive(false);
			Debug.Log("DestroyPrefabInstance" + " " + instance.gameObject.activeSelf);
			stack.Push(instance);
			Debug.Log(stack.Count);
		}
		else
		{
			Debug.Log("DestroyPrefabInstance - Destroy");
			Destroy(instance.gameObject);
		}
	}
}

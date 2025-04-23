using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class UnitPooling : NetworkBehaviour
{

    [SerializeField] private NetworkObject unit;

    private float spawnTimer = 0f;
    private float despawnTimer = 0f;

    private Queue<NetworkObject> spawnedUnits = new Queue<NetworkObject>();

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer == false) return; // 서버에서만 스폰/디스폰 처리

        spawnTimer += Runner.DeltaTime;
        despawnTimer += Runner.DeltaTime;

        // 2초마다 스폰
        if (spawnTimer >= 2f)
        {
            spawnTimer = 0f;

            NetworkObject newUnit = Runner.Spawn(unit, GetRandomSpawnPos(), Quaternion.identity);
            Debug.Log("유닛 스폰" + newUnit.gameObject.name);
            spawnedUnits.Enqueue(newUnit);
        }

        // 4초마다 가장 오래된 유닛 제거
        if (despawnTimer >= 4f)
        {
            despawnTimer = 0f;

            if (spawnedUnits.Count > 0)
            {
                NetworkObject oldest = spawnedUnits.Dequeue();
                Runner.Despawn(oldest);
            }
        }
    }

    private Vector3 GetRandomSpawnPos()
    {
        return new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }
}

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
        if (Runner.IsServer == false) return; // ���������� ����/���� ó��

        spawnTimer += Runner.DeltaTime;
        despawnTimer += Runner.DeltaTime;

        // 2�ʸ��� ����
        if (spawnTimer >= 2f)
        {
            spawnTimer = 0f;

            NetworkObject newUnit = Runner.Spawn(unit, GetRandomSpawnPos(), Quaternion.identity);
            Debug.Log("���� ����" + newUnit.gameObject.name);
            spawnedUnits.Enqueue(newUnit);
        }

        // 4�ʸ��� ���� ������ ���� ����
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

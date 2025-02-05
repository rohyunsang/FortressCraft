using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class MonsterSpawner : NetworkBehaviour
    {
        [SerializeField] NetworkObject Monster;
        [SerializeField] float spawnDelay = 10.0f;
        [SerializeField] int maxSpawnConut = 3;
        public int SpawnCount { get; set; }
        private TickTimer spawnTimer;


        public override void Spawned()
        {
            SpawnCount = 0;
            spawnTimer = TickTimer.CreateFromSeconds(Runner, 0.01f);

            RPCCheckSpawnCount(SpawnCount);
        }

        // FixedUpdateNetwork는 Athority 있는 거에서만 돌아서 FixedUpdate에서 처리 필요 
        private void FixedUpdate()
        {
            if (Runner == null) return;
            //Debug.Log("Count: " + SpawnCount);
            if (spawnTimer.Expired(Runner))
            {
                spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnDelay);

                if (!Runner.IsSharedModeMasterClient) return;
                else Object.RequestStateAuthority();

                if (SpawnCount >= maxSpawnConut) return;

                MonsterController monster = SpawnMonster();
                ++SpawnCount;
                RPCSetSpawnCount(SpawnCount);
                monster.Spawner = this;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCCheckSpawnCount(int cnt)
        {
            if( SpawnCount != cnt )
            {
                RPCSetSpawnCount(SpawnCount);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetSpawnCount(int cnt)
        {
            SpawnCount = cnt;
        }

        private MonsterController SpawnMonster()
        {
            NetworkObject monster = Runner.Spawn(Monster, (Vector2)transform.position, Quaternion.identity);
            return monster.GetComponent<MonsterController>();
        }
    }
}



using Fusion;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class NormalUnitGenerator : NetworkBehaviour
    {
        private TickTimer spawnTimer { get; set; }
        public float SpawnTime { get; set; }
        NormalUnitSpawner spawner;

        public override void Spawned()
        {
            spawner = GetComponent<NormalUnitSpawner>();
            SpawnTime = NormalUnitDataManager.Instance.SpawnTime;
            if (spawner.Usable) SetWaitingTime();
        }

        public override void FixedUpdateNetwork()
        {
            //Debug.Log("Generating");
            //Debug.Log( Id );
            if (spawnTimer.Expired(Runner))
            {
                //Debug.Log(Id);
                SetWaitingTime();
                spawner.SpawnUnit();
            }
        }

        private void SetWaitingTime()
        {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, SpawnTime);
        }
    }
}
using Fusion;
using UnityEngine;

public class NormalUnitGenerator : NetworkBehaviour
{
    private TickTimer spawnTimer { get; set; }
    [SerializeField] int spwanTime = 5;
    NormalUintSpawner spawner;

    public override void Spawned()
    {
        spawner = GetComponent<NormalUintSpawner>();
        if( spawner.Usable ) SetWaitingTime();
    }

    public override void FixedUpdateNetwork()
    {
        //Debug.Log("Generating");
        //Debug.Log( Id );
        if (spawnTimer.Expired(Runner))
        {
            //Debug.Log(Id);
            SetWaitingTime();
            spawner.SpawnUnit(Runner);
        }
    }

    private void SetWaitingTime()
    {
        spawnTimer = TickTimer.CreateFromSeconds(Runner, spwanTime);
    }
}

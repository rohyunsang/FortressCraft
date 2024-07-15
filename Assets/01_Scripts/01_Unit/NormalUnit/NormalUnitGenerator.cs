using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NormalUnitGenerator : NetworkBehaviour
{
    private TickTimer spawnTimer { get; set; }
    [SerializeField] int spwanTime = 5;
    NormalUintSpawner spawner;

    public override void Spawned()
    {
        spawner = GetComponent<NormalUintSpawner>();
        SetWaitingTime();
    }

    public override void FixedUpdateNetwork()
    {
        //Debug.Log("Generating");
        //Debug.Log( Id );
        if (spawnTimer.Expired(Runner))
        {
            //Debug.Log(Id);
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spwanTime);
            spawner.SpawnUnit(Runner);
        }
    }

    private void SetWaitingTime()
    {
        spawnTimer = TickTimer.CreateFromSeconds(Runner, spwanTime);
    }
}

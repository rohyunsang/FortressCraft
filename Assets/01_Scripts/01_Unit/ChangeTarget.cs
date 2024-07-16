using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTarget : NetworkBehaviour
{
    private NormalUintSpawner[] spawners;
    private NormalUintSpawner targetSpawner;
    private Button[] btns;

    private void Awake()
    {
        spawners = GameObject.FindObjectsOfType<NormalUintSpawner>();

        btns = GetComponentsInChildren<Button>();

        foreach (Button btn in btns)
        {
            if (btn.transform.name == "Button_A")
            {
                btn.onClick.AddListener(setTargetA);
            }
            else if (btn.transform.name == "Button_B")
            {
                btn.onClick.AddListener(setTargetB);
            }
            else if (btn.transform.name == "Button_C")
            {
                btn.onClick.AddListener(setTargetC);
            }
            else if (btn.transform.name == "Button_D")
            {
                btn.onClick.AddListener(setTargetD);
            }
        }

    }

    private void FixedUpdate()
    {
        if (targetSpawner == null) SettingSpawner();
    }

    private void SettingSpawner()
    {
        foreach (NormalUintSpawner spawner in spawners)
        {
            if (spawner.player == null) continue;

            if (spawner.player.PlayerId.PlayerId == Runner.LocalPlayer.PlayerId)
            {
                targetSpawner = spawner;

                break;
            }
        }
    }

    

    public void setTargetA()
    {
        if (targetSpawner == null) return;
        Debug.Log("Set as A");
        targetSpawner.RPCTargetChange("A");
        /*
        foreach (NormalUintSpawner spawner in spawners)
        {
            Debug.Log("Set as A");
            spawner.RPCTargetChange("A");
        }
        */
    }


    public void setTargetB()
    {
        if (targetSpawner == null) return;
        Debug.Log("Set as B");
        targetSpawner.RPCTargetChange("B");
        /*
        foreach (NormalUintSpawner spawner in spawners)
        {
            Debug.Log("Set as B");
            spawner.RPCTargetChange("B");
        }
        */
    }

    public void setTargetC()
    {
        if (targetSpawner == null) return;
        Debug.Log("Set as C");
        targetSpawner.RPCTargetChange("C");
        /*
        foreach (NormalUintSpawner spawner in spawners)
        {
            Debug.Log("Set as C");
            spawner.RPCTargetChange("C");
        }
        */
    }

    public void setTargetD()
    {
        if (targetSpawner == null) return;
        Debug.Log("Set as D");
        targetSpawner.RPCTargetChange("D");
        /*
        foreach (NormalUintSpawner spawner in spawners)
        {
            Debug.Log("Set as D");
            spawner.RPCTargetChange("D");
        }
        */
    }

}

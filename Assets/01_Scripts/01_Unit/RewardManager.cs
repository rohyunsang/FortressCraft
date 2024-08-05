using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; set; }
    public int Gold { get; set; }
    public int Exp { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        // Debug.Log("Gold: " + Gold + " Exp: " + Exp);
    }
}

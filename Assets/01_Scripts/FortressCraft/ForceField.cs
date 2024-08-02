using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    // Force Field Destroy

    public void Start()
    {
        Destroy(gameObject, 10f);
    }
}

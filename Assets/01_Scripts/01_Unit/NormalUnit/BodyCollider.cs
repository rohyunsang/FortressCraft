using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollider : MonoBehaviour
{
    public float Damaged { get; set; }

    private void Awake()
    {
        Damaged = 0.0f;
    }
}

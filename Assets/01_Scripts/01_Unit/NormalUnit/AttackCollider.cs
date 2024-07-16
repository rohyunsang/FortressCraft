using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] protected float _damage;
    public string TargetUnit { get; set; }
}

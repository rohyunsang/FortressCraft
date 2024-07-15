using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitFire : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    public Transform TargetTranform { get; set; }
    public string TargetUnit { get; set; }
    private NormalUnit normalUnit;

    private void Awake()
    {
        normalUnit = GetComponent<NormalUnit>();
    }

    public void Fire()
    {
        TargetUnit = normalUnit.TargetUnit;
        GameObject gameObject = Instantiate(arrow, transform).gameObject;

        gameObject.GetComponent<Arrow>().TargetTransform = TargetTranform;
        gameObject.GetComponentInChildren<AttackCollider>().TargetUnit = TargetUnit;
    }
}

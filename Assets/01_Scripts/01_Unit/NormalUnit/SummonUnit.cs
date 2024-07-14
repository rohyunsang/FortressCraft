using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonUnit : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;

    private void Summon()
    {
        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
        Instantiate(unitPrefab, transform.position + offset, Quaternion.identity);
    }

    IEnumerator SummonUnitCoroutine()
    {
        while (true)
        {
            Summon();
            yield return new WaitForSeconds(5f);
        }
    }


    void OnEnable()
    {
        StartCoroutine(SummonUnitCoroutine());
    }


    void OnDisable()
    {
        StopCoroutine(SummonUnitCoroutine());
    }
}

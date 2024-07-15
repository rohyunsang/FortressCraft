using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private float _damage;
    public string TargetUnit { get; set; }
    private Arrow arrow;

    private void Awake()
    {
        arrow = GetComponent<Arrow>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag(TargetUnit) )
        {
            if(collision.TryGetComponent<BodyCollider>(out BodyCollider bodycoliider))
            {
                bodycoliider.Damaged = _damage;
                Debug.Log("Hit!");
                arrow.DestroySelf();
            }
        }
    }
}

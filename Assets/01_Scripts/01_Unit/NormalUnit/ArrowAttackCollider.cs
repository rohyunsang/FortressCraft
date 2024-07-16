using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackCollider : AttackCollider
{
    private Arrow arrow;

    private void Awake()
    {
        arrow = GetComponent<Arrow>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TargetUnit))
        {
            if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycoliider))
            {
                bodycoliider.Damaged = _damage;
                //Debug.Log("Hit!");
                arrow.DestroySelf();
            }
            
        }
    }
}

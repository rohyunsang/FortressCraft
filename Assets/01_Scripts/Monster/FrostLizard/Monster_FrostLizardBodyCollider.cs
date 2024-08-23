using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class Monster_FrostLizardBodyCollider : BodyCollider
    {
        Monster_FrostLizardController controller;

        private void Awake()
        {
            controller = transform.parent.GetComponent<Monster_FrostLizardController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Attack"))
            {
                if (collision.TryGetComponent<AttackCollider>(out AttackCollider attackCollider))
                {
                    controller.RPCCheckDamage();
                }
            }
        }
    }
}



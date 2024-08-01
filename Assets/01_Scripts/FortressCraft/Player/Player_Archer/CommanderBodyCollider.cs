using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CommanderBodyCollider : BodyCollider
    {
        Player player;

        private void Awake()
        {
            player = transform.parent.GetComponent<Player>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if( collision.CompareTag("Attack") )
            {
                if( collision.TryGetComponent<AttackCollider>(out AttackCollider attackCollider) )
                {
                    player.RPCCheckDamaged();
                }
            }
        }
    }
}


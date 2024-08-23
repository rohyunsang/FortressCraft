using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class Monster_FrostLizardAttackCollider : AttackCollider
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.StartsWith("Unit"))
            {
                if (collision.TryGetComponent<BodyCollider>(out BodyCollider bodycollider))
                {
                    bodycollider.RPCSetDamage(Damage);
                    bodycollider.CallDamageCheck();
                }
            }
        }
    }
}



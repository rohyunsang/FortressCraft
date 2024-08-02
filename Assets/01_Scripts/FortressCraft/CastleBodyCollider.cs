using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBodyCollider : BodyCollider
    {
        Castle castle;

        public override void Spawned()
        {
            base.Spawned();
            castle = transform.parent.GetComponent<Castle>();
            transform.tag = "Unit_" + castle.team;
        }
        /*
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if( Damaged >= 0.0f )
            {
                castle.Damage(Damaged);
            }
        }
        */
    }
}



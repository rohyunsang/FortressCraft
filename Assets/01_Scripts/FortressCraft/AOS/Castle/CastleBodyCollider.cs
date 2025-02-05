using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBodyCollider : BodyCollider
    {
        public Castle castle;
        public CastleManager castleManager;

        public override void Spawned()
        {
            base.Spawned();

            switch( castle.team )
            {
                case Team.A:
                    transform.tag = "Unit_A";
                    break;
                case Team.B:
                    transform.tag = "Unit_B";
                    break;
                case Team.C:
                    transform.tag = "Unit_C";
                    break;
                case Team.D:
                    transform.tag = "Unit_D";
                    break;
            }

        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Attack"))
            {
                if (collision.TryGetComponent<AttackCollider>(out AttackCollider attackCollider))
                {
                    castleManager.RPCCheckDamaged();
                }
            }
        }
    }
}



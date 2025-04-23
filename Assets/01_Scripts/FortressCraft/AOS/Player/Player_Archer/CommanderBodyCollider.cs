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

        public override void CallDamageCheck()
        {
            player.RPCCheckDamaged();
        }
    }
}


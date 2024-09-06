using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class MonsterBodyCollider : BodyCollider
    {
        MonsterController controller;

        private void Awake()
        {
            controller = transform.parent.GetComponent<MonsterController>();
        }

        public override void CallDamageCheck()
        {
            controller.RPCCheckDamaged();
        }
    }
}



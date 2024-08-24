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

        public override void CallDamageCheck()
        {
            controller.RPCCheckDamaged();
        }
    }
}



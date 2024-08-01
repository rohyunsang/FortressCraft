using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class BodyCollider : NetworkBehaviour
    {
        public float Damaged { get; set; }

        private void Awake()
        {
            Damaged = 0.0f;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetDamage(float damage)
        {
            Damaged = damage;
        }
    }
}


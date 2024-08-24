using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

namespace Agit.FortressCraft
{
    public class MagicianTwinkle : NetworkBehaviour
    {
        public NetworkPrefabId ID { get; set; }
        private TickTimer onOfftimer;
        private TickTimer destroyTimer;
        private BoxCollider2D trigger;


        public override void Spawned()
        {
            trigger = GetComponentInChildren<BoxCollider2D>();
            onOfftimer = TickTimer.CreateFromSeconds(Runner, 0.01f);
            destroyTimer = TickTimer.CreateFromSeconds(Runner, 5.0f);
        }

        public override void FixedUpdateNetwork()
        {
            if( destroyTimer.Expired( Runner ) )
            {
                Destroy(this.gameObject);
            }

            if( onOfftimer.Expired(Runner) )
            {
                trigger.enabled = !trigger.enabled;
                onOfftimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
            }

        }
    }
}



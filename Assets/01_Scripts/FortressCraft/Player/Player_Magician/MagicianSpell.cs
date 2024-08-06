using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

namespace Agit.FortressCraft
{
    public class MagicianSpell : NetworkBehaviour
    {
        public Transform Target { get; set; }
        private NetworkRigidbody2D _rb;
        public float SpellSpeed { get; set; }
        public NetworkPrefabId ID { get; set; }
        public bool Fired { get; set; }
        private TickTimer destroyTimer;
        private MagicianSpellAttackCollider attackCollider;


        public override void Spawned()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
            attackCollider = GetComponent<MagicianSpellAttackCollider>();
            Target = null;
            Invoke("DestroySelf", 1.3f);
        }

        public override void FixedUpdateNetwork()
        {
            Target = null;

            Collider2D[] cols = Physics2D.OverlapCircleAll(
                        new Vector2(transform.position.x, transform.position.y), 2.0f);

            foreach (Collider2D col in cols)
            {
                if (col.tag.StartsWith("Unit"))
                {
                    if (col.CompareTag("Unit_" + attackCollider.OwnType)) continue;
                    Target = col.transform.parent;
                    break;
                }
            }

            if (Target != null)
            {
                _rb.Rigidbody.velocity = ( Target.position - transform.position ).normalized * SpellSpeed;
            }
            else
            {
                Debug.Log("Magic Can't Move");
                _rb.Rigidbody.velocity = Vector2.zero;
            }

        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

namespace Agit.FortressCraft
{
    public class ArcherArrow : NetworkBehaviour
    {
        public Vector2 FireDirection { get; set; }
        private NetworkRigidbody2D _rb;
        [SerializeField] private float arrowSpeed = 5.0f;
        public NetworkPrefabId ID { get; set; }
        public bool Fired { get; set; }
        private TickTimer destroyTimer;

        public override void Spawned()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
            Invoke("DestroySelf", 0.8f);
        }

        public override void FixedUpdateNetwork()
        {
            float angle = Mathf.Atan2(FireDirection.y, FireDirection.x) * Mathf.Rad2Deg;
            Quaternion FireRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = FireRotation;

            _rb.Rigidbody.velocity = FireDirection * arrowSpeed;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}



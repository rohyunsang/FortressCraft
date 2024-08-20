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
        private ArcherArrowAttackCollider attackCollider;

        public override void Spawned()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
            attackCollider = GetComponent<ArcherArrowAttackCollider>();
            //Invoke("DestroySelf", 0.8f);
        }

        public override void FixedUpdateNetwork()
        {
            if (attackCollider.OwnType == null) return;
            if (destroyTimer.Expired(Runner))
            {
                if (gameObject.activeSelf == true)
                {
                    Release();
                }
            }

            float angle = Mathf.Atan2(FireDirection.y, FireDirection.x) * Mathf.Rad2Deg;
            Quaternion FireRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = FireRotation;

            _rb.Rigidbody.velocity = FireDirection * arrowSpeed;
        }

        public void ReserveRelease()
        {
            destroyTimer = TickTimer.CreateFromSeconds(Runner, 0.8f);
        }

        public void Release()
        {
            destroyTimer = TickTimer.None;
            NetworkObjectReleaseContext context = new NetworkObjectReleaseContext(Object, ID, false, false);
            NetworkObjectPoolManager.Instance.ReleaseInstance(Runner, context);
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetUnactive()
        {
            gameObject.SetActive(false);
        }
    }
}



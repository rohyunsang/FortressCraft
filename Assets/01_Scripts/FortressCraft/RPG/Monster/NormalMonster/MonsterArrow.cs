using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;
using static UnityEngine.EventSystems.PointerEventData;

namespace Agit.FortressCraft
{
    public class MonsterArrow : NetworkBehaviour
    {
        public Transform TargetTransform { get; set; }
        private NetworkRigidbody2D _rb;
        [SerializeField] private float arrowSpeed = 5.0f;

        public override void Spawned()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (Runner == null) return;
            if (!Runner.IsSharedModeMasterClient) return;
            if (TargetTransform == null) return;

            if ( Mathf.Abs(TargetTransform.position.x - transform.position.x) < 0.05f &&
                 Mathf.Abs(TargetTransform.position.y - transform.position.y) < 0.05f )
            {
                _rb.Rigidbody.velocity = Vector2.zero;
                return;
            }

            Vector3 movDir = TargetTransform.position - transform.position;
            Vector3 movDirNormalized = movDir.normalized;
            _rb.Rigidbody.velocity = movDirNormalized * arrowSpeed;

            float angle = Mathf.Atan2(movDir.y, movDir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = targetRotation;
        }

        public void ReserveRelease()
        {
            //destroyTimer = TickTimer.CreateFromSeconds(Runner, 1.3f);
            Invoke("DestroySelf", 1.3f);
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}
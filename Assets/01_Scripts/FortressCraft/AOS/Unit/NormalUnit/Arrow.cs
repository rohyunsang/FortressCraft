using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;
using static UnityEngine.EventSystems.PointerEventData;

namespace Agit.FortressCraft
{
    public class Arrow : NetworkBehaviour
    {
        public Transform TargetTransform { get; set; }
        private NetworkRigidbody2D _rb;
        [SerializeField] private float arrowSpeed = 5.0f;
        public NetworkPrefabId ID { get; set; }
        public NormalUnitRigidBodyMovement Normal { get; set; }
        public bool Fired { get; set; }
        private TickTimer destroyTimer;

        public override void Spawned()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public override void FixedUpdateNetwork()
        {
            if (TargetTransform == null) return;
            if( destroyTimer.Expired(Runner) )
            {
                if( gameObject.activeSelf == true )
                {
                    Release();
                }
            }

            if (Mathf.Abs(TargetTransform.position.x - transform.position.x) < 0.05f &&
                Mathf.Abs(TargetTransform.position.y - transform.position.y) < 0.05f)
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
            destroyTimer = TickTimer.CreateFromSeconds(Runner, 1.3f);
            //Invoke("DestroySelf", 1.3f);
        }

        public void Release()
        {
            destroyTimer = TickTimer.None;
            NetworkObjectReleaseContext context = new NetworkObjectReleaseContext(Object, ID, false, false);
            NetworkObjectPoolManager.Instance.ReleaseInstance(Runner, context);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetActive()
        {
            gameObject.SetActive(true);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetUnactive()
        {
            gameObject.SetActive(false);
        }
    }
}
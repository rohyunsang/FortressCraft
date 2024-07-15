using UnityEngine;
using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;
using static UnityEngine.EventSystems.PointerEventData;

public class Arrow : NetworkBehaviour
{
    public Transform TargetTransform { get; set; }
    private NetworkRigidbody2D _rb;
    [SerializeField] private float arrowSpeed = 5.0f;

    public override void Spawned()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        Invoke("DestroySelf", 1.3f);
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 movDir = TargetTransform.position - transform.position;
        Vector3 movDirNormalized = movDir.normalized;
        _rb.Rigidbody.velocity = movDirNormalized * arrowSpeed;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }


}

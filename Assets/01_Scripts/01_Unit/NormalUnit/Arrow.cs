using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform TargetTransform { get; set; }
    private Rigidbody2D rb;
    [SerializeField] private float arrowSpeed = 5.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroySelf", 1.3f);
    }

    private void FixedUpdate()
    {
        Vector3 movDir = TargetTransform.position - transform.position;
        Vector3 movDirNormalized = movDir.normalized;
        rb.velocity = movDirNormalized * arrowSpeed;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject camera;
    public CinemachineVirtualCamera vCam;

    [SerializeField] Transform playerCameraRoot;
    private bool followPlayer = true; // ????? ???? ??

    private Vector3 dragOrigin; // ??? ?? ??
    private bool isDragging = false; // ??? ??

    private Coroutine followPlayerCoroutine; // ??? Follow? ???? ?? ???
    NetworkObject thisObject;
    void Start()
    {
        thisObject = GetComponent<NetworkObject>();
        if (thisObject.HasStateAuthority)
        {
            camera = GameObject.Find("Virtual Camera");
            vCam = camera.GetComponent<CinemachineVirtualCamera>();
            vCam.Follow = playerCameraRoot;
        }
    }

    void Update()
    {
        if (!thisObject.HasStateAuthority) return;

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 2) // ??? ?? ??? ???, ??? ?????.
        {
            dragOrigin = Input.mousePosition; // ??? ?? ?? ??
            isDragging = true;

            // ??? ???? ??? ? ??? ???? ??
            if (followPlayerCoroutine != null)
            {
                StopCoroutine(followPlayerCoroutine);
                followPlayerCoroutine = null;
            }
        }

        if (Input.GetMouseButtonUp(0)) // ??? ?? ??? ??
        {
            isDragging = false;

            followPlayerCoroutine = StartCoroutine(EnableFollowPlayerAfterDelay(0.5f));
        }

        if (isDragging) // ??? ???
        {
            Vector3 difference = Input.mousePosition - dragOrigin; // ??? ?? ??? ??? ??
            dragOrigin = Input.mousePosition; // ??? ?? ????
            MoveCamera(difference); // ??? ??
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        followPlayer = false; // ???? ???? ??
        vCam.Follow = null; // ??? follow ?? ??

        // ??? ??? ???? - delta ?? ?? ???? ?? ??
        vCam.transform.position -= Camera.main.ScreenToViewportPoint(new Vector3(delta.x * (19 / 9), delta.y, 0f)) * 3f; // ?? ??? ?? 10? ??
    }
    private IEnumerator EnableFollowPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDragging) // ???? ???? ????
        {
            followPlayer = true;
            vCam.Follow = playerCameraRoot; // ??? follow ??? ????? ??
        }
    }
}
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

    void Start()
    {
        NetworkObject thisObject = GetComponent<NetworkObject>();
        if (thisObject.HasStateAuthority)
        {
            camera = GameObject.Find("Virtual Camera");
            vCam = camera.GetComponent<CinemachineVirtualCamera>();
            vCam.Follow = playerCameraRoot;
        }
    }

}

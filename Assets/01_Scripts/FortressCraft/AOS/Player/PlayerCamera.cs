using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour
{
    public GameObject camera;
    public CinemachineVirtualCamera vCam;

    [SerializeField] Transform playerCameraRoot;
    private bool followPlayer = true; // 플레이어를 따라갈지 여부

    private Vector3 dragOrigin; // 드래그 시작 위치
    private bool isDragging = false; // 드래그 상태

    private Coroutine followPlayerCoroutine; // 카메라 Follow를 재개하기 위한 코루틴
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

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 2 && !IsPointerOverUIObject()) // 마우스 왼쪽 버튼을 누르면, 오른쪽 화면에서만.
        {
            dragOrigin = Input.mousePosition; // 드래그 시작 위치 저장
            isDragging = true;

            // 새로운 드래그가 시작될 때 기존의 코루틴을 중지
            if (followPlayerCoroutine != null)
            {
                StopCoroutine(followPlayerCoroutine);
                followPlayerCoroutine = null;
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼을 떼면
        {
            isDragging = false;

            followPlayerCoroutine = StartCoroutine(EnableFollowPlayerAfterDelay(0.5f));
        }

        if (isDragging) // 드래그 중이면
        {
            Vector3 difference = Input.mousePosition - dragOrigin; // 원점과 현재 마우스 위치의 차이
            dragOrigin = Input.mousePosition; // 드래그 원점 업데이트
            MoveCamera(difference); // 카메라 이동
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        followPlayer = false; // 플레이어 따라가기 중지
        vCam.Follow = null; // 카메라 follow 대상 해제

        // 카메라 위치를 업데이트 - delta 값을 화면 해상도에 맞게 조정
        vCam.transform.position -= Camera.main.ScreenToViewportPoint(new Vector3(delta.x * (19 / 9), delta.y, 0f)) * 3f; // 속도 조절을 위해 10을 곱함
    }
    private IEnumerator EnableFollowPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDragging) // 드래그가 재개되지 않았다면
        {
            followPlayer = true;
            vCam.Follow = playerCameraRoot; // 카메라 follow 대상을 플레이어로 설정
        }
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

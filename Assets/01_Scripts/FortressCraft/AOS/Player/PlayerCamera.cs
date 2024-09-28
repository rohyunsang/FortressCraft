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
    private bool followPlayer = true; // �÷��̾ ������ ����

    private Vector3 dragOrigin; // �巡�� ���� ��ġ
    private bool isDragging = false; // �巡�� ����

    private Coroutine followPlayerCoroutine; // ī�޶� Follow�� �簳�ϱ� ���� �ڷ�ƾ
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

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 2 && !IsPointerOverUIObject()) // ���콺 ���� ��ư�� ������, ������ ȭ�鿡����.
        {
            dragOrigin = Input.mousePosition; // �巡�� ���� ��ġ ����
            isDragging = true;

            // ���ο� �巡�װ� ���۵� �� ������ �ڷ�ƾ�� ����
            if (followPlayerCoroutine != null)
            {
                StopCoroutine(followPlayerCoroutine);
                followPlayerCoroutine = null;
            }
        }

        if (Input.GetMouseButtonUp(0)) // ���콺 ���� ��ư�� ����
        {
            isDragging = false;

            followPlayerCoroutine = StartCoroutine(EnableFollowPlayerAfterDelay(0.5f));
        }

        if (isDragging) // �巡�� ���̸�
        {
            Vector3 difference = Input.mousePosition - dragOrigin; // ������ ���� ���콺 ��ġ�� ����
            dragOrigin = Input.mousePosition; // �巡�� ���� ������Ʈ
            MoveCamera(difference); // ī�޶� �̵�
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        followPlayer = false; // �÷��̾� ���󰡱� ����
        vCam.Follow = null; // ī�޶� follow ��� ����

        // ī�޶� ��ġ�� ������Ʈ - delta ���� ȭ�� �ػ󵵿� �°� ����
        vCam.transform.position -= Camera.main.ScreenToViewportPoint(new Vector3(delta.x * (19 / 9), delta.y, 0f)) * 3f; // �ӵ� ������ ���� 10�� ����
    }
    private IEnumerator EnableFollowPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDragging) // �巡�װ� �簳���� �ʾҴٸ�
        {
            followPlayer = true;
            vCam.Follow = playerCameraRoot; // ī�޶� follow ����� �÷��̾�� ����
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

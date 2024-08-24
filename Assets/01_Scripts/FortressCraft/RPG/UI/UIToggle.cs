using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // �κ��丮 �гο� ���� ����
    public GameObject equipmentPanel; // ��� �гο� ���� ����

    void Update()
    {
        // "I" Ű�� ������ �� �κ��丮 â ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
        // "E" Ű�� ������ �� ��� â ���
        else if (Input.GetKeyDown(KeyCode.E))
        {
            equipmentPanel.SetActive(!equipmentPanel.activeSelf);
        }
    }
}

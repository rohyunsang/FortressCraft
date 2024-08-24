using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // 인벤토리 패널에 대한 참조
    public GameObject equipmentPanel; // 장비 패널에 대한 참조

    void Update()
    {
        // "I" 키를 눌렀을 때 인벤토리 창 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
        // "E" 키를 눌렀을 때 장비 창 토글
        else if (Input.GetKeyDown(KeyCode.E))
        {
            equipmentPanel.SetActive(!equipmentPanel.activeSelf);
        }
    }
}

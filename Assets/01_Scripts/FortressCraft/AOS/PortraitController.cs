using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour
{
    // Image ������Ʈ ������ ���� ������
    public Image warriorPortrait;
    public Image archerPortrait;
    public Image magicianPortrait;

    // ���õ� �̹����� ���� �̹������� ������ ����
    private Color selectedColor = Color.white;  // FFFFFF
    private Color nonSelectedColor = new Color32(0x8A, 0x8A, 0x8A, 0xFF);  // 8A8A8A

    void Start()
    {
        SelectPortrait("Archer");
    }

    // ���õ� �ʻ�ȭ�� ������Ʈ�ϴ� �޼���
    public void SelectPortrait(string portrait)  //using Button
    {
        switch (portrait)
        {
            case "Warrior":
                SetPortraitColors(warriorPortrait, archerPortrait, magicianPortrait);
                break;
            case "Archer":
                SetPortraitColors(archerPortrait, warriorPortrait, magicianPortrait);
                break;
            case "Magician":
                SetPortraitColors(magicianPortrait, warriorPortrait, archerPortrait);
                break;
        }
    }

    // ���� �̹��� ������ �����ϴ� �޼���
    private void SetPortraitColors(Image selected, Image nonSelected1, Image nonSelected2)
    {
        selected.color = selectedColor;
        nonSelected1.color = nonSelectedColor;
        nonSelected2.color = nonSelectedColor;
    }
}

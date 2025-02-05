using Agit.FortressCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour
{
    // Image 컴포넌트 참조를 위한 변수들
    public Image warriorPortrait;
    public Image archerPortrait;
    public Image magicianPortrait;
    public Image greatSwordPortrait;

    // 선택된 이미지와 비선택 이미지들의 색상을 정의
    private Color selectedColor = Color.white;  // FFFFFF
    private Color nonSelectedColor = new Color32(0x8A, 0x8A, 0x8A, 0xFF);  // 8A8A8A

    void Start()
    {
        SelectPortrait("Archer");
    }

    // 선택된 초상화를 업데이트하는 메서드
    public void SelectPortrait(string portrait)  //using Button
    {
        switch (portrait)
        {
            case "Warrior":
                SetPortraitColors(warriorPortrait, archerPortrait, magicianPortrait, greatSwordPortrait);
                break;
            case "Archer":
                SetPortraitColors(archerPortrait, warriorPortrait, magicianPortrait, greatSwordPortrait);
                break;
            case "Magician":
                SetPortraitColors(magicianPortrait, warriorPortrait, archerPortrait, greatSwordPortrait);
                break;
            case "GreatSword":
                UIManager.Instance._unImplementInfo.SetActive(true);
                
                // 미구현이라 아처로 대신. 
                SetPortraitColors(archerPortrait, warriorPortrait, magicianPortrait, greatSwordPortrait); 

                // SetPortraitColors(greatSwordPortrait, warriorPortrait, archerPortrait, magicianPortrait);
                break;
        }
    }

    // 실제 이미지 색상을 설정하는 메서드
    private void SetPortraitColors(Image selected, Image nonSelected1, Image nonSelected2, Image nonSelected3)
    {
        selected.color = selectedColor;
        nonSelected1.color = nonSelectedColor;
        nonSelected2.color = nonSelectedColor;
        nonSelected3.color = nonSelectedColor;
    }
}

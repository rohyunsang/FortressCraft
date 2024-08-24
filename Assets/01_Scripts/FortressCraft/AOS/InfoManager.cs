using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public Text infoText;
    public List<string> infoTexts = new List<string>();

    private int currentTextIndex = 0;
    private float textChangeInterval = 7.0f; // 텍스트 변경 간격 (초)
    private float lastTextChangeTime; // 마지막으로 텍스트 변경한 시각

    public void Awake()
    {
        infoTexts.Add("왼쪽 하단을 클릭하면 조이스틱이 생성됩니다.!");
        infoTexts.Add("조이스틱을 이용해 캐릭터를 움직여 보세요.");
        infoTexts.Add("직업은 전사, 궁수, 마법사로 이루어져 있습니다.");
        infoTexts.Add("점령 가능 지역을 점령해 영토를 확장하세요.");
        infoTexts.Add("2명 이상의 플레이어가 모이면 게임을 시작할 수 있습니다.");
        infoTexts.Add("상대의 점령지역을 모두 파괴하면 게임에서 승리합니다.");

        lastTextChangeTime = Time.time; // 초기화
        UpdateText(); // 최초 텍스트 업데이트
    }

    void Update()
    {
        if (Time.time - lastTextChangeTime >= textChangeInterval)
        {
            UpdateText(); // 텍스트 업데이트
            lastTextChangeTime = Time.time; // 시간 업데이트
        }
    }

    void UpdateText()
    {
        if (infoTexts.Count > 0)
        {
            infoText.text = infoTexts[currentTextIndex]; // 현재 인덱스의 텍스트를 UI에 설정
            currentTextIndex = (currentTextIndex + 1) % infoTexts.Count; // 다음 텍스트 인덱스로 이동
        }
    }
}
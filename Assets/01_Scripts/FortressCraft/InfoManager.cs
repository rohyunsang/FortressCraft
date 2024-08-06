using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public Text infoText;
    public List<string> infoTexts = new List<string>();

    private int currentTextIndex = 0;
    private float textChangeInterval = 7.0f; // �ؽ�Ʈ ���� ���� (��)
    private float lastTextChangeTime; // ���������� �ؽ�Ʈ ������ �ð�

    public void Awake()
    {
        infoTexts.Add("������ ����, �ü�, ������� �̷���� �ֽ��ϴ�.");
        infoTexts.Add("���� ���� ������ ������ ���並 Ȯ���ϼ���.");
        infoTexts.Add("4���� �÷��̾ �����ϸ� ������ ������ �� �ֽ��ϴ�.");
        infoTexts.Add("����� ���������� ��� �ı��ϸ� ���ӿ��� �¸��մϴ�.");

        lastTextChangeTime = Time.time; // �ʱ�ȭ
        UpdateText(); // ���� �ؽ�Ʈ ������Ʈ
    }

    void Update()
    {
        if (Time.time - lastTextChangeTime >= textChangeInterval)
        {
            UpdateText(); // �ؽ�Ʈ ������Ʈ
            lastTextChangeTime = Time.time; // �ð� ������Ʈ
        }
    }

    void UpdateText()
    {
        if (infoTexts.Count > 0)
        {
            infoText.text = infoTexts[currentTextIndex]; // ���� �ε����� �ؽ�Ʈ�� UI�� ����
            currentTextIndex = (currentTextIndex + 1) % infoTexts.Count; // ���� �ؽ�Ʈ �ε����� �̵�
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Agit.FortressCraft
{
    public class ChatSystem : MonoBehaviour
    {
        [Header("Objects")]
        public GameObject chatEntryCanvas;
        public InputField chatInputField;
        public Text chatDisplay;
        public string playerName = "";

        public static ChatSystem instance;

        private void Awake()
        {
            // �̱��� �ν��Ͻ��� �̹� �����Ѵٸ�, ���� ������Ʈ�� �ı�
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            // �̱��� �ν��Ͻ��� �� ������Ʈ�� ����
            instance = this;

            // �ɼ�: �� ���� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
            DontDestroyOnLoad(this.gameObject);
        }

        public void SendButton()
        {
            Player[] players = FindObjectsOfType<Player>();

            foreach (Player player in players)
            {
                if (player != null && player.PlayerName.ToString() == playerName)
                {
                    player.ChatGate();
                    break;
                }
                else
                {
                    Debug.Log("Player ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            
        }


    }

}
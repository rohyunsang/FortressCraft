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
            // 싱글톤 인스턴스가 이미 존재한다면, 현재 컴포넌트를 파괴
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            // 싱글톤 인스턴스를 이 컴포넌트로 설정
            instance = this;

            // 옵션: 이 게임 오브젝트가 씬 전환 시 파괴되지 않도록 설정
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
                    Debug.Log("Player 컴포넌트를 찾을 수 없습니다.");
                }
            }
            
        }


    }

}
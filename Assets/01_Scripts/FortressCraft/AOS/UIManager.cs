using Agit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Agit.FortressCraft 
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; } // Singleton instance

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Prevent the singleton object from being destroyed on load
            }
            else
            {
                Destroy(gameObject); // Destroy this instance if a duplicate exists
            }

            joinButton.onClick.AddListener(InitOnClickJoinButton);
        }

        #region Login
        public GameObject _loginScreen;
        public GameObject _nicknameGroup;
        public GameObject _successMakeNicknameGroup;
        public GameObject _gameStartButton;
        public Text _nicknameText;  // profile
        public GameObject _failNicknameInfo;
        public Text _outputText; // failNickNameInfo - Text


        #endregion

        #region UI - INTRO

        public InputField _room;
        public Button createButton;
        public Button joinButton;
        [FormerlySerializedAs("_nicknamePanel")] // 이전 변수명 지정
        public GameObject _createRoomPanel; // 새 변수명으로 변경
        public GameObject _incorrectNicknamePanel;
        public GameObject _roomListPanel;
        public GameObject _roomOptionPanel;

        public void InitOnClickJoinButton()
        {
            _room.text = "";
        }

        #endregion


        #region UI - LOBBY And INGAME
        public Button startButton;
        public Button leaveToSessionButton;
        
        public Button leaveToGameButtonVictoryPanel; // Victory Panel Button;
        public Button leaveToGameButtonDefeatPanel; // Defeat Panel Button;


        public GameObject victoryPanel;
        public GameObject defeatPanel;

        public GameObject darkFilter;

        public GameObject voiceOff;
        public GameObject voiceOn;

        public GameObject LoadingMsg;

        public void Init()
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            voiceOff.SetActive(true);
            voiceOn.SetActive(false);
        }

        public void OnDefeatPanel()
        {
            defeatPanel.SetActive(true);
            BattleBarUIManager.Instance.RPCClearUnitCount(BattleBarUIManager.Instance.OwnType);
        }

        public void OnVictoryPanel()
        {
            victoryPanel.SetActive(true);
        }

        public void OnDarkFilter()
        {
            darkFilter.SetActive(true);
        }

        public void OffDarkFilter()
        {
            darkFilter.SetActive(false);
        }

        #endregion
    }
}


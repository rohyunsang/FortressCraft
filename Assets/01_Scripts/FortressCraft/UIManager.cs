using Agit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace Agit.FortressCraft 
{
    public class UIManager : MonoBehaviour
    {
        #region UI - INTRO

        public InputField _room;
        public InputField _playerName;
        public Button createButton;
        public Button joinButton;
        public GameObject _nicknamePanel;
        public GameObject _incorrectNicknamePanel;
        public GameObject _roomListPanel;
        public GameObject _roomOptionPanel;

        private void Awake()
        {
            joinButton.onClick.AddListener(InitOnClickJoinButton);
            createButton.onClick.AddListener(InitOnClickCreateButton);
        }

        public void InitOnClickJoinButton()
        {
            _room.text = "";
            _playerName.text = "";
        }
        private void InitOnClickCreateButton()
        {
            _playerName.text = "";
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

        public void Init()
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
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


using Agit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace Agit.FortressCraft 
{
    public class UIManager : Singleton<UIManager>
    {
        #region UI - INTRO

        public InputField _room;
        public InputField _playerName;
        public Button joinButton;

        private void Awake()
        {
            joinButton.onClick.AddListener(InitOnClickJoinButton);
        }

        public void InitOnClickJoinButton()
        {
            _room.text = "";
            _playerName.text = "";
        }

        #endregion


        #region UI - LOBBY


        #endregion


        #region UI - IN GAME


        #endregion


    }

}


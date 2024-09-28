using FusionHelpers;
using Photon.Voice.Unity.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft {
    public class SessionEntryPrefab : MonoBehaviour
    {
        public Text roomName;
        public Text playerCount;
        public Button joinButton;

        public void init(string roomName, string playerCount, string maxPlayer)
        {
            this.roomName.text = roomName;
            this.playerCount.text = playerCount + " / " + maxPlayer;
            joinButton.onClick.AddListener(ConnectToSession);
            // FindObjectOfType<ConnectAndJoin>().RoomName = roomCode;
        }

        public void ConnectToSession()
        {
            FindObjectOfType<App>().GetComponent<App>().SetRoomCodeOverride(roomName.text);
            FindObjectOfType<App>().GetComponent<App>().ConnectToSession();
            UIManager.Instance._roomListPanel.SetActive(false);
        }
    }
}



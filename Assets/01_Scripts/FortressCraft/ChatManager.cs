using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using PhotonAppSettings = global::Fusion.Photon.Realtime.PhotonAppSettings;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using TMPro;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        [SerializeField] private PhotonAppSettings photonSettings;

        bool isConnected = false;

        ChatClient chatClient;

        public string userID = "";
        public string roomCode = "";

        public string privateReceiver = "";

        public Text chatDisplay;
        [SerializeField] private TMP_InputField chatField;
        private string currentChat = "";
        public Button sendButton;

        // Start is called before the first frame update
        public void Init()  
        {

            chatClient = new ChatClient(this);
            var settings = photonSettings.AppSettings;

            chatClient.Connect(settings.AppIdChat, settings.AppVersion, new AuthenticationValues(userID));
            Debug.Log("userID " + userID);
            chatClient.Subscribe(new string[] { "1234" });

            sendButton.onClick.AddListener(SubmitPublicChat);
        }

        // Update is called once per frame
        void Update()
        {

            if (isConnected)
            {
                chatClient.Service();
            }
        }

        public void SubmitPublicChat()
        {
            if (privateReceiver == "")
            {
                chatClient.PublishMessage(roomCode, chatField.text); // Make sure the correct channel is used
                Debug.Log($"Message published to {roomCode}: {chatField.text}");
                currentChat = "";
                chatField.text = "";
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
        }

        public void OnChatStateChange(ChatState state)
        {
        }

        public void OnConnected()
        {
            isConnected = true;

            Debug.Log("Connected to Photon Chat");
        }

        public void OnDisconnected()
        {
            isConnected = false;

            Debug.Log("Disconnected from Photon Chat");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            if (messages.Length > 0)
            {
                Debug.Log($"Received messages on {channelName}");
                string msgs = "";
                for (int i = 0; i < senders.Length; i++)
                {
                    msgs += $"{senders[i]}: {messages[i].ToString()}, ";
                }
                chatDisplay.text += "\n" + msgs;
                Debug.Log(msgs);
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                if (results[i])
                {
                    Debug.Log($"Successfully subscribed to {channels[i]}");
                }
                else
                {
                    Debug.Log($"Failed to subscribe to {channels[i]}");
                }
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
        }

        public void OnUserSubscribed(string channel, string user)
        {
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
        }


    }

}

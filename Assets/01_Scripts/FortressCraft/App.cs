using Fusion;
using Agit.UIHelpers;
using FusionHelpers;
using Tanknarok.UI;
using TMPro;
using UnityEngine;
using System;
using static UnityEngine.Random;
using Random = UnityEngine.Random;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine.UI;
using Photon.Realtime;

namespace Agit.FortressCraft
{
	/// <summary>
	/// App entry point and main UI flow management.
	/// </summary>
	public class App : MonoBehaviour
	{
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private GameManager _gameManagerPrefab;
		[SerializeField] private InputField _room;
        [SerializeField] private InputField _playerName;
        [SerializeField] private InputField _playerNameOverride;
        [SerializeField] private TextMeshProUGUI _progress;
		[SerializeField] private Panel _uiStart;
		[SerializeField] private Panel _uiProgress;
		[SerializeField] private GameObject _uiLevel;
		[SerializeField] private TMP_Dropdown _regionDropdown;

		[SerializeField] private GameObject selectJoinModePanel;
		[SerializeField] private GameObject nickNamePanel;
		[SerializeField] private GameObject roomListPanel;


        public string roomCode = "";
		public void SetRoomCodeOverride(string roomCodeOverride)
		{
			this.roomCode = roomCodeOverride;
			_levelManager.roomCode.text = "Room : " + roomCodeOverride;
        }

        private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;
		private GameMode _gameMode;


		private void Awake()
		{
			Application.targetFrameRate = 60;
			DontDestroyOnLoad(this);
			_levelManager.onStatusUpdate = OnConnectionStatusUpdate;

            SetGameMode(GameMode.Shared);
        }

		private void Start()
		{
			OnConnectionStatusUpdate( null, FusionLauncher.ConnectionStatus.Disconnected, "");
		}

		private void Update()
		{
		}

		public void ShutDownSession() // using button 
		{
			FindObjectOfType<FusionLauncher>().ShutDownCustom();
        }

		public void ConnectToLobby() // using Button
		{

            FusionLauncher.ConnectToLobby(_playerNameOverride.text, _gameManagerPrefab, OnConnectionStatusUpdate);
        }

		public void ConnectToSession()
		{
            // Get region from dropdown
            string region = string.Empty;
            if (_regionDropdown.value > 0)
            {
                region = _regionDropdown.options[_regionDropdown.value].text;
                region = region.Split(" (")[0];
            }

            FusionLauncher.ConnectToSession(region, _levelManager, roomCode);

            // UI SetActive false
			selectJoinModePanel.SetActive(false);
            nickNamePanel.SetActive(false);
            roomListPanel.SetActive(false);
        }

        public void SetRoomName()  // using    App - UI Intro - RoomOptionPanel - Launch 
        {
			_levelManager.roomCode.text = "Room : " + _room.text;
			roomCode = _room.text;

            SetVoiceRoomName();
        }

        public void CreateRoom()  // using  App - UI Intro - Start Panel - CreateRoom
        {
            CreateRandomRoomCode();
        }

        private void CreateRandomRoomCode()
		{
            // 0에서 99999 사이의 랜덤 숫자 생성
            int randomNumber = Random.Range(0, 100000);
            // 숫자를 문자열로 변환하면서 5자리 포맷을 유지
            string randomCode = randomNumber.ToString("D5");
			
			roomCode = randomCode;
            _room.text = randomCode;

            _levelManager.SetRoomCode(_room.text);
        }

		private void SetVoiceRoomName()
		{
			GameObject recorder = GameObject.FindGameObjectWithTag("Recorder");
			if (recorder != null)
			{
				recorder.GetComponent<ConnectAndJoin>().RoomName = roomCode;
			}
		}

		public void OnCancel()
        {
			_uiStart.SetVisible(true);
        }

		private void SetGameMode(GameMode gamemode)
		{
			_gameMode = gamemode;
		}

		public void OnEnterRoom()
		{
			// Get region from dropdown
			string region = string.Empty;
			if (_regionDropdown.value > 0)
            {
				region = _regionDropdown.options[_regionDropdown.value].text;
				region = region.Split(" (")[0];
            }
            FusionLauncher.Launch(_gameMode, region, _room.text, _playerName.text, _gameManagerPrefab, _levelManager, OnConnectionStatusUpdate);
		}

		private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
		{
			if (!this)
				return;

			Debug.Log(status);

			if (status != _status)
			{
				switch (status)
				{
					case FusionLauncher.ConnectionStatus.Disconnected:
						ErrorBox.Show("Disconnected!", reason, () => { });
						break;
					case FusionLauncher.ConnectionStatus.Failed:
						ErrorBox.Show("Error!", reason, () => { });
						break;
					case FusionLauncher.ConnectionStatus.Loaded:
						break;
				}
			}

			_status = status;
			UpdateUI();
		}

		private void UpdateUI()
		{
			bool intro = false;
			bool progress = false;
			bool running = false;

			switch (_status)
			{
				case FusionLauncher.ConnectionStatus.Disconnected:
					_progress.text = "Disconnected!";
					intro = true;
					break;
				case FusionLauncher.ConnectionStatus.Failed:
					_progress.text = "Failed!";
					intro = true;
					break;
				case FusionLauncher.ConnectionStatus.Connecting:
					_progress.text = "Connecting";
					progress = true;
					break;
				case FusionLauncher.ConnectionStatus.Connected:
					_progress.text = "Connected";
					progress = true;
					break;
				case FusionLauncher.ConnectionStatus.Loading:
					_progress.text = "Loading";
					progress = true;
					break;
				case FusionLauncher.ConnectionStatus.Loaded:
					running = true;
					break;
			}

			_uiStart.SetVisible(intro);
			_uiProgress.SetVisible(progress);
			_uiLevel.SetActive(running);
		}
	}
}
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
        [SerializeField] private InputField playerName;
        [SerializeField] private TextMeshProUGUI _progress;
		[SerializeField] private Panel _uiStart;
		[SerializeField] private Panel _uiProgress;
		[SerializeField] private GameObject _uiRoom;
		[SerializeField] private GameObject _uiGame;
		[SerializeField] private TMP_Dropdown _regionDropdown;


		public string roomCode = "";


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
			if (_uiProgress.isShowing)
			{
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					NetworkRunner runner = FindObjectOfType<NetworkRunner>();
					if (runner != null && !runner.IsShutdown)
					{
						// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
						runner.Shutdown(false);
					}
				}
				UpdateUI();
			}
		}

		public void SetRoomName()  // using    App - UI Intro - RoomOptionPanel - Launch 
        {
			_levelManager.roomCodeTMP.text = "Room Code : " + _room.text;
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

			Debug.Log(_room.text);

            _levelManager.SetRoomCode(_room.text);

			// SetVoiceRoomName();

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

			FusionLauncher.Launch(_gameMode, region, _room.text, playerName.text, _gameManagerPrefab, _levelManager, OnConnectionStatusUpdate);
		}

		/// <summary>
		/// Call this method from button events to close the current UI panel and check the return value to decide
		/// if it's ok to proceed with handling the button events. Prevents double-actions and makes sure UI panels are closed. 
		/// </summary>
		/// <param name="ui">Currently visible UI that should be closed</param>
		/// <returns>True if UI is in fact visible and action should proceed</returns>
		private bool GateUI(Panel ui)
		{
			if (!ui.isShowing)
				return false;
			ui.SetVisible(false);
			return true;
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
                        // _chatManager.Init();
						break;
				}
			}

			_status = status;
			UpdateUI();
		}

		public void ToggleAudio()
        {
			// AudioListener.volume = 1f - AudioListener.volume;
			//_audioText.text = AudioListener.volume > 0.5f ? "ON" : "OFF";
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
			_uiGame.SetActive(running);
			
		}
	}
}
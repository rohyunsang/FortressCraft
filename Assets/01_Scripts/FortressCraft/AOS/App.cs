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
using Photon.Voice.Unity.Demos.DemoVoiceUI;

namespace Agit.FortressCraft
{
	public enum Mode
	{
        Survival,
		Team
    }


	public class App : MonoBehaviour
	{
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private GameManager _gameManagerPrefab;
		[SerializeField] private InputField _room;
        [SerializeField] private TextMeshProUGUI _progress;
		[SerializeField] private Panel _uiStart;
		[SerializeField] private Panel _uiProgress;
		[SerializeField] private GameObject _uiLevel;
		[SerializeField] private TMP_Dropdown _regionDropdown;

		[SerializeField] private GameObject selectJoinModePanel;
		[SerializeField] private GameObject nickNamePanel;
		[SerializeField] private GameObject roomListPanel;

		[SerializeField] private GameObject[] warriorSkillUIs;
		[SerializeField] private GameObject[] archerSkillUIs;
		[SerializeField] private GameObject[] magicianSkillUIs;


        public string roomCode = "";

		public Mode mode;

        private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;
		private GameMode _gameMode;
		
		public JobType jobType;

		public GameObject recorderPrefab;

        private void Awake()
		{
			Application.targetFrameRate = 60;
			DontDestroyOnLoad(this);
			_levelManager.onStatusUpdate = OnConnectionStatusUpdate;

            SetGameMode(GameMode.Shared);
        }

		private void Start()
		{
			OnConnectionStatusUpdate(null, FusionLauncher.ConnectionStatus.Disconnected, "");

			StartCoroutine(GoogleSheetManager.Loader());

			this.jobType = JobType.Archer; // job type default 
            JobUIVisible(archerSkillUIs);
        }


		public void SetJob(string jobType)   // using Button ;
		{
			switch (jobType)
			{
				case "Warrior":
                    this.jobType = JobType.Warrior;
					JobUIVisible(warriorSkillUIs);
                    break;
				case "Archer":
                    this.jobType = JobType.Archer;
					JobUIVisible(archerSkillUIs);
                    break;
				case "Magician":
                    this.jobType = JobType.Magician;
					JobUIVisible(magicianSkillUIs);
                    break;
			}
		}

		private void JobUIVisible(GameObject[] UIs)
		{
			foreach(GameObject UI in warriorSkillUIs)
			{
				UI.gameObject.SetActive(false);
			}
            foreach (GameObject UI in archerSkillUIs)
            {
                UI.gameObject.SetActive(false);
            }
            foreach (GameObject UI in magicianSkillUIs)
            {
                UI.gameObject.SetActive(false);
            }

            foreach (GameObject UI in UIs)
			{
				UI.gameObject.SetActive(true);
			}
		}


		public void SetGameModeSurvival() // using Button 
		{
			mode = Mode.Survival;
			Debug.Log(mode);
		}
        public void SetGameModeTeam()
        {
			mode = Mode.Team;
			Debug.Log(mode);
        }

        public void SetRoomCodeOverride(string roomCodeOverride)
        {
            this.roomCode = roomCodeOverride;
            _levelManager.roomCode.text = "Room : " + roomCodeOverride;
        }
        public void ShutDownSession() // using button 
		{
			FindObjectOfType<FusionLauncher>().ShutDownCustom();
        }

        public void CreateRoom()
        {
            CreateRandomRoomCode();
            OnEnterRoom();
            SetRoomName();
            FindObjectOfType<UIManager>()._createRoomPanel.SetActive(false);
        }

		public void JoinRoom()
		{
            OnEnterRoom();
            SetRoomName();
            FindObjectOfType<UIManager>()._roomOptionPanel.SetActive(false);
        }

        public void ConnectToLobby() 
		{
			FindObjectOfType<UIManager>().Init();
            FindObjectOfType<LevelUIController>().Init();

			string nickname = FindObjectOfType<NicknameManager>().nickname;

            FusionLauncher.ConnectToLobby(nickname, _gameManagerPrefab, OnConnectionStatusUpdate);
        }

		public void ConnectToSession()
		{
            Instantiate(recorderPrefab).gameObject.name = "[Recorder]";
            SetVoiceRoomName();

            GameObject bgm = FindObjectOfType<BGM>().gameObject;
            if (bgm != null)
                Destroy(FindObjectOfType<BGM>().gameObject);

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
            roomListPanel.SetActive(false);
        }

        public void SetRoomName()  // using    App - UI Intro - RoomOptionPanel - Launch 
        {
			_levelManager.roomCode.text = "Room : " + _room.text;
			roomCode = _room.text;
        }

        private void CreateRandomRoomCode()
		{
            int randomNumber = Random.Range(0, 100000);
            string randomCode = randomNumber.ToString("D5");
			
			roomCode = randomCode;
            _room.text = randomCode;
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
            FindObjectOfType<UIManager>().Init();
            FindObjectOfType<LevelUIController>().Init();

			GameObject bgm = FindObjectOfType<BGM>().gameObject;
			if(bgm != null)
				Destroy(FindObjectOfType<BGM>().gameObject);

            Instantiate(recorderPrefab).gameObject.name = "[Recorder]";
            SetVoiceRoomName();
            // Get region from dropdown
            string region = string.Empty;
			if (_regionDropdown.value > 0)
            {
				region = _regionDropdown.options[_regionDropdown.value].text;
				region = region.Split(" (")[0];
            }

            string nickname = FindObjectOfType<NicknameManager>().nickname;

            FusionLauncher.Launch(_gameMode, region, _room.text, nickname, _gameManagerPrefab, _levelManager, OnConnectionStatusUpdate);
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

	public enum JobType
	{
		Warrior = 0,
		Archer,
		Magician,
        GreatSword,
    }
}
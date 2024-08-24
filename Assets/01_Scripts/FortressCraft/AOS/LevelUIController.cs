using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class LevelUIController : MonoBehaviour
    {
        // Only Lobby
        [SerializeField] private GameObject lobbyParent;
        [SerializeField] private GameObject leaveToSessionButton;
        [SerializeField] private GameObject disconnectPrompt;

        // Both
        [SerializeField] private GameObject bothParent;


        // InGame
        [SerializeField] private GameObject inGameParent;

        // RPG
        [SerializeField] private GameObject _rpgParent;

        private void Start()
        {
            lobbyParent.SetActive(true);
            leaveToSessionButton.SetActive(true);
            bothParent.SetActive(true);
        }

        public void Init()
        {
            lobbyParent.SetActive(true);
            bothParent.SetActive(true);
            leaveToSessionButton.SetActive(true);
            inGameParent.SetActive(false);
            disconnectPrompt.SetActive(false);
        }

        public void BattleSceneUIChange()
        {
            leaveToSessionButton.SetActive(false);
            lobbyParent.SetActive(false);
            inGameParent.SetActive(true);

            BattleBarUIManager.Instance.SetBattleBar();
        }
        
        public void RPGSceneUIChange()
        {
            _rpgParent.SetActive(true);
        }
    }
}
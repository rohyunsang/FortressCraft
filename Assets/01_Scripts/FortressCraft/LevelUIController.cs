using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    // Only Lobby
    [SerializeField] private GameObject lobbyParent;
    [SerializeField] private GameObject leaveToSessionButton;

    // Both
    [SerializeField] private GameObject bothParent;

    // InGame
    [SerializeField] private GameObject inGameParent;

    private void Start()
    {
        lobbyParent.SetActive(true);
        leaveToSessionButton.SetActive(true);
        bothParent.SetActive(true);
    }

    public void BattleSceneUIChange()
    {
        leaveToSessionButton.SetActive(false);
        lobbyParent.SetActive(false);
        inGameParent.SetActive(true);
    }
}

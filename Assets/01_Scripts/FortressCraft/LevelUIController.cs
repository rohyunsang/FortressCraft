using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyParent;

    [SerializeField] private GameObject bothParent;

    [SerializeField] private GameObject inGameParent;

    private void Start()
    {
        lobbyParent.SetActive(true);
        bothParent.SetActive(true);
    }

    public void BattleSceneUIChange()
    {
        lobbyParent.SetActive(false);
        inGameParent.SetActive(true);
    }
}

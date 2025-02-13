using Agit.FortressCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestLogin : MonoBehaviour
{
    public Button _guestLoginButton;
    public GameObject _loginPanel;
    public GameObject _nicknamePanel; 

    private void Start()
    {
        _guestLoginButton.onClick.AddListener(OnClickGuesLoginButton);
    }

    private void OnClickGuesLoginButton()
    {
        string guestNickname = "Guest" + Random.Range(1000, 9999).ToString();

        FindObjectOfType<NicknameManager>().nickname = guestNickname;
        UIManager.Instance._nicknameText.text = guestNickname;


        _loginPanel.SetActive(false);
        _nicknamePanel.SetActive(false);


    }


}

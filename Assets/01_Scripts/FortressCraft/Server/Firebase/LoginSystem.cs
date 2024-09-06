using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class LoginSystem : MonoBehaviour
    {
        public InputField email;
        public InputField password;

        public Text outputText;

        void Start()
        {
            FirebaseAuthManager.Instance.LoginState += OnChangedState;
            FirebaseAuthManager.Instance.Init();
            FirebaseDBManager.Instance.Init();
        }

        private void OnChangedState(bool sign)
        {
            // outputText.text = sign ? "�α��� : " : "�α׾ƿ� : ";
            if (sign)
            {
                outputText.text = "회원 가입이 완료됐습니다.";
            }
        }

        public void Create()
        {
            FirebaseAuthManager.Instance.Create(email.text, password.text);
        }

        public void LogIn()
        {
            FirebaseAuthManager.Instance.Login(email.text, password.text);
        }

        public void LogOut()
        {
            FirebaseAuthManager.Instance.LogOut();
        }

    }

}

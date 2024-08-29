using UnityEngine;
using Firebase.Auth;
using System.IO;
using Firebase;
using Firebase.Extensions;
using System;
using Firebase.Database;

namespace Agit.FortressCraft {
    public class FirebaseAuthManager
    {
        private static FirebaseAuthManager instance = null;

        public static FirebaseAuthManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FirebaseAuthManager();
                }

                return instance;
            }
        }

        private FirebaseAuth auth;
        private FirebaseUser user;

        public Action<bool> LoginState;

        public string UserId => user.UserId;

        public void Init()
        {
            auth = FirebaseAuth.DefaultInstance;

            // 임시처리
            if(auth.CurrentUser != null)
            {
                LogOut();
            }

            auth.StateChanged += OnChanged;
        }
        private void OnChanged(object sender, EventArgs e)
        {
            if (auth.CurrentUser != user)
            {
                bool signed = (auth.CurrentUser != user && auth.CurrentUser != null);
                if (!signed && user != null)
                {
                    Debug.Log("로그아웃");
                    LoginState?.Invoke(false);
                }
                user = auth.CurrentUser;

                if (signed)
                {
                    Debug.Log("로그인");
                    LoginState?.Invoke(true);
                }
            }
        }

        public void Create(string email, string password)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("회원가입 취소");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("회원가입 실패");
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                Debug.Log("회원가입 완료");
                MainThreadDispatcher.Enqueue(() =>
                {
                    FirebaseDBManager.Instance.WriteNewUser(UserId);
                });
            });
        }


        public void Login(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("로그인 취소");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("로그인 실패");
                    return;
                }

                FirebaseUser newUser = task.Result.User;

                Debug.Log("로그인 완료");

                // 메인 스레드에서 UI 업데이트 실행
                MainThreadDispatcher.Enqueue(() =>
                {
                    UIManager.Instance._loginScreen.SetActive(false);
                });
            });
        }
        public void LogOut()
        {
            auth.SignOut();
            Debug.Log("로그아웃");
        }
    }

}

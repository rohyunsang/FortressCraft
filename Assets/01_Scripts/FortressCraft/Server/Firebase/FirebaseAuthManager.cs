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

            // �ӽ�ó��
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
                    Debug.Log("�α׾ƿ�");
                    LoginState?.Invoke(false);
                }
                user = auth.CurrentUser;

                if (signed)
                {
                    Debug.Log("�α���");
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
                    Debug.Log("ȸ������ ���");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("ȸ������ ����");
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                Debug.Log("ȸ������ �Ϸ�");
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
                    Debug.Log("�α��� ���");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("�α��� ����");
                    return;
                }

                FirebaseUser newUser = task.Result.User;

                Debug.Log("�α��� �Ϸ�");

                // ���� �����忡�� UI ������Ʈ ����
                MainThreadDispatcher.Enqueue(() =>
                {
                    UIManager.Instance._loginScreen.SetActive(false);
                });
            });
        }
        public void LogOut()
        {
            auth.SignOut();
            Debug.Log("�α׾ƿ�");
        }
    }

}

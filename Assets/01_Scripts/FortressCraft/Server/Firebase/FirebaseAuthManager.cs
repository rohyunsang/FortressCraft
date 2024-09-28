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
                    LoginState?.Invoke(false);
                }
                user = auth.CurrentUser;

                if (signed)
                {
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
                    return;
                }

                if (task.IsFaulted)
                {
                    return;
                }

                FirebaseUser newUser = task.Result.User;
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
                    return;
                }

                if (task.IsFaulted)
                {
                    return;
                }

                FirebaseUser newUser = task.Result.User;

                MainThreadDispatcher.Enqueue(() =>
                {
                    UIManager.Instance._loginScreen.SetActive(false);
                });
            });
        }
        public void LogOut()
        {
            auth.SignOut();
        }
    }

}

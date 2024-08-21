using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using System.IO;
using Firebase;
using Firebase.Extensions;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseAuth user;

    public InputField email;
    public InputField password;

    void Awake()
    {
        if (auth == null)
        {
            auth = FirebaseAuth.DefaultInstance;
        }
    }

    public void Create()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => 
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
        });
    }
    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
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
        });
    }
    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("�α׾ƿ�");
    }
}

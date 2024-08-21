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
        });
    }
    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
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
        });
    }
    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("로그아웃");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Agit.FortressCraft
{

    public class FirebaseDBManager
    {
        private static FirebaseDBManager instance = null;

        public static FirebaseDBManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FirebaseDBManager();
                }

                return instance;
            }
        }

        DatabaseReference database; // 데이터를 쓰기 위한 reference 변수


        public void Init()
        {
            database = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void GetUserPropertiesByUid(string uid, Action<UserProperties> callback)
        {
            DatabaseReference userPropertiesRef = database.Child(uid).Child("UserProperties");
            userPropertiesRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error retrieving user properties: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        UserProperties properties = JsonUtility.FromJson<UserProperties>(snapshot.GetRawJsonValue());
                        callback(properties);
                    }
                }
            });
        }

        public void UpdateGold(string uid, int newGold)
        {
            DatabaseReference goldRef = database.Child(uid).Child("UserProperties").Child("gold");
            goldRef.SetValueAsync(newGold).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update gold: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Gold updated to " + newGold);
                }
            });
        }

        public void UpdateNickname(string uid, string nickName)
        {
            DatabaseReference nickNameRef = database.Child(uid).Child("UserDatas").Child("userNickname");
            nickNameRef.SetValueAsync(nickName).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update nickName: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Nickname updated to " + nickName);
                }
            });
        }

        public Task<bool> IsNicknameAvailable(string nickname)
        {
            DatabaseReference nicknameRef = database.Child("UserIds").Child(nickname);
            var tcs = new TaskCompletionSource<bool>();
            nicknameRef.GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error accessing database");
                    tcs.SetResult(false);
                }
                else if (task.Result.Exists)
                {
                    tcs.SetResult(false); // 이미 존재하면 false
                }
                else
                {
                    tcs.SetResult(true); // 사용 가능하면 true
                }
            });
            return tcs.Task;
        }

        // 사용 가능한 닉네임이면 데이터베이스에 저장
        public Task SetNickname(string nickname, string userId)
        {
            DatabaseReference nicknameRef = database.Child("UserIds").Child(nickname);
            return nicknameRef.SetValueAsync(userId);
        }

        public class UserDatas // 사용자 클래스 생성
        {
            public string userNickname;
            public UserDatas()
            {
                this.userNickname = "user123";
            }
        }
        public class UserProperties
        {
            public int gold;
            public int cash;
            public int level;
            public float experience;

            public UserProperties()
            {
                this.gold = 0;
                this.cash = 0;
                this.level = 1;
                this.experience = 0;
            }
        }
        public void WriteNewUser(string userid) // 가입한 회원 고유 번호에 대한 사용자 기본값 설정
        {
            UserDatas user = new UserDatas();
            string jsonUser = JsonUtility.ToJson(user);
            database.Child(userid).Child("UserDatas").SetRawJsonValueAsync(jsonUser);

            UserProperties userProperty = new UserProperties();
            string jsonProperties = JsonUtility.ToJson(userProperty);
            database.Child(userid).Child("UserProperties").SetRawJsonValueAsync(jsonProperties);
        }
    }
}
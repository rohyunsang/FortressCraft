using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity;
using JetBrains.Annotations;


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

        public void GetUserPropertiesByUid(string uid, System.Action<UserProperties> callback)
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

        public class UserDatas // 사용자 클래스 생성
        {
            public string userNickname;
            public string guildType;
            public string currentMap;
            public UserDatas()
            {
                this.userNickname = "user123";
                this.guildType = "None"; // 기본 값 설정
                this.currentMap = "Starting Area";
            }
        }
        public class UserProperties
        {
            public int gold;
            public int cash;
            public int level;
            public float experience;
            public float hp;
            public float mp;

            public UserProperties()
            {
                this.gold = 0;
                this.cash = 0;
                this.level = 1;
                this.experience = 0;
                this.hp = 50;
                this.mp = 50;
            }
        }
        public class Inventory
        {
            public int maxInventory;
            public List<Item> items;

            public Inventory(int maxInventory)
            {
                this.maxInventory = maxInventory;
                this.items = new List<Item>();
            }

            // 아이템 추가
            public void AddItem(string itemId, int quantity)
            {
                Item item = items.Find(x => x.itemId == itemId);
                if (item != null)
                {
                    item.quantity += quantity;
                }
                else
                {
                    items.Add(new Item(itemId, quantity));
                }
            }
        }

        [System.Serializable]
        public class Item
        {
            public string itemId;
            public int quantity;

            public Item(string itemId, int quantity)
            {
                this.itemId = itemId;
                this.quantity = quantity;
            }
        }

        public void WriteNewUser(string userid) // 가입한 회원 고유 번호에 대한 사용자 기본값 설정
        {
            UserDatas user = new UserDatas();
            string jsonUser = JsonUtility.ToJson(user);
            database.Child(userid).Child("UserData").SetRawJsonValueAsync(jsonUser);

            UserProperties userProperty = new UserProperties();
            string jsonProperties = JsonUtility.ToJson(userProperty);
            database.Child(userid).Child("UserProperties").SetRawJsonValueAsync(jsonProperties);

            Inventory inventory = new Inventory(24);
            string jsonInventory = JsonUtility.ToJson(inventory);
            database.Child(userid).Child("Inventory").SetRawJsonValueAsync(jsonInventory);
        }
    }
}
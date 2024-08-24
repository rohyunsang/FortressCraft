using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using UnityEditor.Search;


namespace Agit.FortressCraft
{
    public enum InventoryType
    {
        Equipment = 0,
        Consumable,
        Misc,
    }

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

        public Inventory equipmentInventory { get; private set; }
        public Inventory consumablesInventory { get; private set; }
        public Inventory miscInventory { get; private set; }

        public void Init()
        {
            database = FirebaseDatabase.DefaultInstance.RootReference;
            InitializeInventories();
        }
        private void InitializeInventories()
        {
            equipmentInventory = new Inventory();
            consumablesInventory = new Inventory();
            miscInventory = new Inventory();
        }


        public void AddItemToInventory(string itemId, int quantity, InventoryType inventoryType)
        {
            Inventory targetInventory = GetInventoryByType(inventoryType);
            if (targetInventory != null)
            {
                targetInventory.AddItem(itemId, quantity);
                UpdateInventoryInDatabase(inventoryType); // Update the specific inventory in the database.
            }
        }

        public void RemoveItemFromInventory(string itemId, int quantity, InventoryType inventoryType)
        {
            Inventory targetInventory = GetInventoryByType(inventoryType);
            if (targetInventory != null)
            {
                targetInventory.RemoveItem(itemId, quantity);
                UpdateInventoryInDatabase(inventoryType); // Update the specific inventory in the database.
            }
        }

        private void UpdateInventoryInDatabase(InventoryType inventoryType)
        {
            Inventory targetInventory = GetInventoryByType(inventoryType);
            if (targetInventory != null)
            {
                string jsonInventory = JsonUtility.ToJson(targetInventory);
                database.Child(FirebaseAuthManager.Instance.UserId).Child(inventoryType.ToString() + "Inventory").SetRawJsonValueAsync(jsonInventory).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Failed to update " + inventoryType.ToString().ToLower() + " inventory: " + task.Exception);
                    }
                    else if (task.IsCompleted)
                    {
                        Debug.Log(inventoryType.ToString() + " inventory updated successfully.");
                    }
                });
            }
        }

        private Inventory GetInventoryByType(InventoryType inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Equipment:
                    return equipmentInventory;
                case InventoryType.Consumable:
                    return consumablesInventory;
                case InventoryType.Misc:
                    return miscInventory;
                default:
                    return null;
            }
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

        public void GetUserDatasByUid(string uid, Action<UserProperties> callback)
        {
            DatabaseReference userDatasRef = database.Child(uid).Child("UserDatas");
            userDatasRef.GetValueAsync().ContinueWith(task =>
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
            DatabaseReference goldRef = database.Child(uid).Child("UserDatas").Child("userNickname");
            goldRef.SetValueAsync(nickName).ContinueWith(task =>
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
            public int currentItemCount;

            // Constructor with a default max inventory size of 24
            public Inventory(int maxInventory = 24)
            {
                this.maxInventory = maxInventory;
                items = new List<Item>();
                currentItemCount = 0;
            }

            public void AddItem(string itemId, int quantity)
            {
                if (currentItemCount + quantity > maxInventory)
                {
                    Debug.LogError("Cannot add item: Inventory capacity exceeded.");
                    return;
                }

                Item item = items.Find(x => x.itemId == itemId);
                if (item != null)
                {
                    item.quantity += quantity;
                }
                else
                {
                    items.Add(new Item(itemId, quantity));
                }
                currentItemCount += quantity;
            }

            public void RemoveItem(string itemId, int quantity)
            {
                Item item = items.Find(x => x.itemId == itemId);
                if (item != null)
                {
                    int newQuantity = item.quantity - quantity;
                    if (newQuantity <= 0)
                    {
                        items.Remove(item);
                        currentItemCount -= item.quantity;
                    }
                    else
                    {
                        item.quantity = newQuantity;
                        currentItemCount -= quantity;
                    }
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
            database.Child(userid).Child("UserDatas").SetRawJsonValueAsync(jsonUser);

            UserProperties userProperty = new UserProperties();
            string jsonProperties = JsonUtility.ToJson(userProperty);
            database.Child(userid).Child("UserProperties").SetRawJsonValueAsync(jsonProperties);

            // Initialize and save each inventory type
            Inventory equipmentInventory = new Inventory(); // Default max is 24
            Inventory consumablesInventory = new Inventory(); // Default max is 24
            Inventory miscInventory = new Inventory(); // Default max is 24

            SaveInventoryToDatabase(userid, "Equipment", equipmentInventory);
            SaveInventoryToDatabase(userid, "Consumables", consumablesInventory);
            SaveInventoryToDatabase(userid, "Misc", miscInventory);
        }
        private void SaveInventoryToDatabase(string userid, string inventoryType, Inventory inventory)
        {
            string jsonInventory = JsonUtility.ToJson(inventory);
            database.Child(userid).Child(inventoryType + "Inventory").SetRawJsonValueAsync(jsonInventory).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save " + inventoryType + " inventory: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log(inventoryType + " inventory saved successfully for user " + userid);
                }
            });
        }
    }
}
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
    public enum InventoryType
    {
        Equipment = 0,
        Consumable,
        Misc,
    }

    public class FirebaseDBManager
    {
        private static FirebaseDBManager instance = null;
        private const int MaxSlots = 24; // 인벤토리 타입별 최대 슬롯 수

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

        public Dictionary<InventoryType, Inventory> inventories = new Dictionary<InventoryType, Inventory>();

        public void Init()
        {
            database = FirebaseDatabase.DefaultInstance.RootReference;
            InitializeInventories();
        }
        private void InitializeInventories()
        {
            inventories[InventoryType.Equipment] = new Inventory(1, 24);
            inventories[InventoryType.Consumable] = new Inventory(100, 24);
            inventories[InventoryType.Misc] = new Inventory(100, 24);
        }

        public void AddItemToInventory(string itemId, int quantity, InventoryType inventoryType)
        {
            Inventory inventory = inventories[inventoryType];
            inventory.AddItem(itemId, quantity);
            UpdateInventoryInDatabase(inventoryType);
        }

        public void UpdateInventoryInDatabase(InventoryType inventoryType)
        {
            Inventory inventory = inventories[inventoryType];
            string jsonInventory = JsonUtility.ToJson(inventory);
            database.Child(FirebaseAuthManager.Instance.UserId).Child($"{inventoryType}Inventory").SetRawJsonValueAsync(jsonInventory);
        }


        public Inventory GetInventoryByType(InventoryType inventoryType)
        {
            if (inventories.TryGetValue(inventoryType, out Inventory inventory))
            {
                return inventory;
            }
            return null; // 없을 경우 null 반환
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
            public int maxItemsPerSlot;
            public List<Item>[] slots;

            public Inventory(int maxPerSlot, int numberOfSlots)
            {
                maxItemsPerSlot = maxPerSlot;
                slots = new List<Item>[numberOfSlots];
                for (int i = 0; i < numberOfSlots; i++)
                {
                    slots[i] = new List<Item>();
                }
            }

            public void AddItem(string itemId, int quantity)
            {
                foreach (List<Item> slot in slots)
                {
                    Item existingItem = slot.FirstOrDefault(item => item.itemId == itemId);
                    if (existingItem != null && existingItem.quantity + quantity <= maxItemsPerSlot)
                    {
                        existingItem.quantity += quantity;
                        return;
                    }
                }

                // No existing item found or slot max reached; try to add to a new slot
                foreach (List<Item> slot in slots)
                {
                    if (slot.Count == 0 || slot.Sum(item => item.quantity) < maxItemsPerSlot)
                    {
                        slot.Add(new Item(itemId, Math.Min(quantity, maxItemsPerSlot)));
                        return;
                    }
                }

                Debug.Log("No available slot or all slots are full.");
            }

            public bool CanAddItem(string itemId, int quantity)
            {
                foreach (List<Item> slot in slots)
                {
                    Item existingItem = slot.FirstOrDefault(item => item.itemId == itemId);
                    if (existingItem != null && existingItem.quantity + quantity <= maxItemsPerSlot)
                    {
                        return true;
                    }
                }

                // Check if there is an empty slot or a slot with enough space to add the new item
                foreach (List<Item> slot in slots)
                {
                    if (slot.Count == 0 || slot.Sum(item => item.quantity) + quantity <= maxItemsPerSlot)
                    {
                        return true;
                    }
                }

                return false;
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

            // 각 인벤토리 유형에 대해 인벤토리 데이터를 저장
            foreach (var inventoryEntry in inventories)
            {
                SaveInventoryToDatabase(userid, inventoryEntry.Key.ToString(), inventoryEntry.Value);
            }
        }

        private void SaveInventoryToDatabase(string userId, string inventoryType, Inventory inventory)
        {
            string jsonInventory = JsonUtility.ToJson(inventory);
            database.Child(userId).Child(inventoryType + "Inventory").SetRawJsonValueAsync(jsonInventory).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to save {inventoryType} inventory: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log($"{inventoryType} inventory saved successfully for user {userId}");
                }
            });
        }
    }
}
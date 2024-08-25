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
        private const int MaxSlots = 24; // �κ��丮 Ÿ�Ժ� �ִ� ���� ��

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

        DatabaseReference database; // �����͸� ���� ���� reference ����

        public Dictionary<InventoryType, Inventory> inventories = new Dictionary<InventoryType, Inventory>();

        public void Init()
        {
            database = FirebaseDatabase.DefaultInstance.RootReference;
            InitializeInventories();
        }
        private void InitializeInventories()
        {
            inventories[InventoryType.Equipment] = new Inventory(1, 24);
            inventories[InventoryType.Consumable] = new Inventory(10, 24);
            inventories[InventoryType.Misc] = new Inventory(10, 24);
        }

        public void AddItemToInventory(string itemId, int quantity, InventoryType inventoryType, Action<bool> callback)
        {
            Inventory inventory = inventories[inventoryType];
            int? slotIndex = inventory.AddItem(itemId, quantity);  // �������� �߰��ϰ�, ����� ������ �ε����� ��ȯ����

            if (slotIndex.HasValue)
            {
                // �����ͺ��̽��� Ư�� �κ��丮 ���� ������Ʈ
                UpdateInventoryInDatabase(inventory.slots[slotIndex.Value], inventoryType, slotIndex.Value);
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        public void UpdateInventoryInDatabase(Slot slot, InventoryType inventoryType, int slotIndex)
        {
            string jsonInventory = JsonUtility.ToJson(slot);
            if (jsonInventory == null)
            {
                Debug.LogError("Failed to serialize slot");
                return;
            }

            database.Child(FirebaseAuthManager.Instance.UserId)
                    .Child($"{inventoryType}Inventory")
                    .Child("Slots")
                    .Child(slotIndex.ToString())
                    .SetRawJsonValueAsync(jsonInventory);
        }


        public Inventory GetInventoryByType(InventoryType inventoryType)
        {
            if (inventories.TryGetValue(inventoryType, out Inventory inventory))
            {
                return inventory;
            }
            return null; // ���� ��� null ��ȯ
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
                    tcs.SetResult(false); // �̹� �����ϸ� false
                }
                else
                {
                    tcs.SetResult(true); // ��� �����ϸ� true
                }
            });
            return tcs.Task;
        }

        // ��� ������ �г����̸� �����ͺ��̽��� ����
        public Task SetNickname(string nickname, string userId)
        {
            DatabaseReference nicknameRef = database.Child("UserIds").Child(nickname);
            return nicknameRef.SetValueAsync(userId);
        }

        public class UserDatas // ����� Ŭ���� ����
        {
            public string userNickname;
            public string guildType;
            public string currentMap;
            public UserDatas()
            {
                this.userNickname = "user123";
                this.guildType = "None"; // �⺻ �� ����
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
        [System.Serializable]
        public class Slot
        {
            public List<Item> items;

            public Slot()
            {
                items = new List<Item>();
            }
        }

        [System.Serializable]
        public class Inventory
        {
            public int maxItemsPerSlot;
            public List<Slot> slots;

            public Inventory(int maxPerSlot, int numberOfSlots)
            {
                maxItemsPerSlot = maxPerSlot;
                slots = new List<Slot>();
                for (int i = 0; i < numberOfSlots; i++)
                {
                    slots.Add(new Slot());
                }
            }

            public string Serialize()
            {
                return JsonUtility.ToJson(this);
            }

            public int? AddItem(string itemId, int quantity)
            {
                // ���� ��� ������ �˻��Ͽ� ������ �߰� �õ�
                foreach (var slot in slots)
                {
                    Item item = slot.items.FirstOrDefault(x => x.itemId == itemId);
                    if (item != null)
                    {
                        // �̹� �����ϴ� �����ۿ� �߰�
                        if (item.quantity + quantity <= maxItemsPerSlot)
                        {
                            item.quantity += quantity;
                            return slots.IndexOf(slot); // �������� �߰��� ������ �ε��� ��ȯ
                        }
                    }
                }

                // ������ ������ ���� ��� ���ο� �������� �� ���Կ� �߰�
                foreach (var slot in slots)
                {
                    if (slot.items.Count == 0 || slot.items.Sum(x => x.quantity) < maxItemsPerSlot)
                    {
                        slot.items.Add(new Item(itemId, Math.Min(quantity, maxItemsPerSlot)));
                        return slots.IndexOf(slot); // �� �������� �߰��� ������ �ε��� ��ȯ
                    }
                }

                // ��� ������ ���� �� �ִ� ��� null ��ȯ
                return null;
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

        public void WriteNewUser(string userid) // ������ ȸ�� ���� ��ȣ�� ���� ����� �⺻�� ����
        {
            UserDatas user = new UserDatas();
            string jsonUser = JsonUtility.ToJson(user);
            database.Child(userid).Child("UserDatas").SetRawJsonValueAsync(jsonUser);

            UserProperties userProperty = new UserProperties();
            string jsonProperties = JsonUtility.ToJson(userProperty);
            database.Child(userid).Child("UserProperties").SetRawJsonValueAsync(jsonProperties);

            // �� �κ��丮 ������ ���� �κ��丮 �����͸� ����
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
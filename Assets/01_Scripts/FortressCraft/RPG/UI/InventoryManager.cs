using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft {

    public class InventoryManager : MonoBehaviour
    {
        public Transform equipmentInventoryContent; // 인벤토리 UI의 Content 영역
        public Transform consumableInventoryContent;
        public Transform miscInventoryContent;
        public GameObject itemPrefab; // 아이템을 표현할 프리팹
        public static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 게임오브젝트를 씬 로딩 시 파괴하지 않도록 설정
            }
            else
            {
                Destroy(gameObject); // 이미 인스턴스가 생성된 경우 중복 생성된 객체를 파괴
            }
        }

        private Transform GetInventoryContent(InventoryType inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Equipment:
                    return equipmentInventoryContent;
                case InventoryType.Consumable:
                    return consumableInventoryContent;
                case InventoryType.Misc:
                    return miscInventoryContent;
                default:
                    return null;
            }
        }

        // Adds an item to the correct UI inventory based on the type
        public void AddItemToGameInventory(string itemId, int quantity, InventoryType inventoryType)
        {
            Transform inventoryContent = GetInventoryContent(inventoryType);
            if (inventoryContent == null)
            {
                Debug.LogError("Invalid inventory type provided.");
                return;
            }
            if(inventoryType != InventoryType.Equipment)
            {
                // Find existing item by ID
                foreach (Transform itemTransform in inventoryContent)
                {
                    Text itemText = itemTransform.GetComponentInChildren<Text>();
                    if (itemText && itemText.text.StartsWith(itemId))
                    {
                        string[] parts = itemText.text.Split(':');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int currentQuantity))
                        {
                            int newQuantity = currentQuantity + quantity;
                            itemText.text = $"{itemId}: {newQuantity}";
                            Debug.Log($"Updated {itemId} quantity to {newQuantity} in {inventoryType} inventory.");
                            return;
                        }
                    }
                }
            }

            // If item not found, add new item
            GameObject item = Instantiate(itemPrefab, inventoryContent);
            item.GetComponentInChildren<Text>().text = $"{itemId}: {quantity}";
            Debug.Log($"Added new item {itemId} with quantity {quantity} to {inventoryType} inventory.");
        }

        public void RemoveItemFromGameInventory(string itemId, InventoryType inventoryType)
        {
            Transform inventoryContent = GetInventoryContent(inventoryType);
            foreach (Transform child in inventoryContent)
            {
                if (child.GetComponentInChildren<Text>().text.StartsWith(itemId))
                {
                    Destroy(child.gameObject);
                    Debug.Log($"Item {itemId} removed from {inventoryType} inventory.");
                    break;
                }
            }
        }

        // 아이템을 데이터베이스에 추가합니다.
        public void AddItemToDatabase(string itemId, int quantity, InventoryType inventoryType)
        {
            FirebaseDBManager.Instance.AddItemToInventory(itemId, quantity, inventoryType);
        }

        // 아이템을 데이터베이스에서 제거합니다.
        public void RemoveItemFromDatabase(string itemId, int quantity, InventoryType inventoryType)
        {
            // FirebaseDBManager.Instance.RemoveItemFromInventory(itemId, quantity, inventoryType);
        }

        public void AddItemTestButton()
        {
            AddItemToDatabase("sword3", 1, InventoryType.Equipment);  // 예를 들어, "sword" 아이템을 하나 추가합니다.
        }

        public void DeleteItemTestButton()
        {
            RemoveItemFromDatabase("sword", 1, InventoryType.Equipment);  // "sword" 아이템을 하나 제거합니다.

        }
    }
}


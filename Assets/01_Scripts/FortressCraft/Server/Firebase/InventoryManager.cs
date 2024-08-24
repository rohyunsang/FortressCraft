using UnityEngine;

namespace Agit.FortressCraft {

    public class InventoryManager : MonoBehaviour
    {
        // 아이템을 데이터베이스에 추가합니다.
        public void AddItemToDatabase(string itemId, int quantity)
        {
            FirebaseDBManager.Instance.AddItemToInventory(itemId, quantity, InventoryType.Equipment);
        }

        // 아이템을 데이터베이스에서 제거합니다.
        public void RemoveItemFromDatabase(string itemId, int quantity)
        {
            FirebaseDBManager.Instance.RemoveItemFromInventory(itemId, quantity, InventoryType.Equipment);
        }

        public void AddItemTestButton()
        {
            AddItemToDatabase("sword3", 1);  // 예를 들어, "sword" 아이템을 하나 추가합니다.
        }

        public void DeleteItemTestButton()
        {
            RemoveItemFromDatabase("sword", 1);  // "sword" 아이템을 하나 제거합니다.

        }
    }
}


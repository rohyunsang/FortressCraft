using UnityEngine;

namespace Agit.FortressCraft {

    public class InventoryManager : MonoBehaviour
    {
        // �������� �����ͺ��̽��� �߰��մϴ�.
        public void AddItemToDatabase(string itemId, int quantity)
        {
            FirebaseDBManager.Instance.AddItemToInventory(itemId, quantity, InventoryType.Equipment);
        }

        // �������� �����ͺ��̽����� �����մϴ�.
        public void RemoveItemFromDatabase(string itemId, int quantity)
        {
            FirebaseDBManager.Instance.RemoveItemFromInventory(itemId, quantity, InventoryType.Equipment);
        }

        public void AddItemTestButton()
        {
            AddItemToDatabase("sword3", 1);  // ���� ���, "sword" �������� �ϳ� �߰��մϴ�.
        }

        public void DeleteItemTestButton()
        {
            RemoveItemFromDatabase("sword", 1);  // "sword" �������� �ϳ� �����մϴ�.

        }
    }
}


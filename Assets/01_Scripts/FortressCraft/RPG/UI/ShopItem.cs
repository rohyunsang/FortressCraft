using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static Agit.FortressCraft.FirebaseDBManager;

namespace Agit.FortressCraft
{
    public class ShopItem : MonoBehaviour
    {
        public string _itemName;
        public int _itemPrice;
        public Button _purchaseButton;
        public InventoryType inventoryType;

        void Start()
        {
            _purchaseButton.onClick.AddListener(OnClickPurchaseButton);
        }

        private void OnClickPurchaseButton()
        {
            Debug.Log("Purchase button clicked");
            string uid = FirebaseAuthManager.Instance.UserId;
            Debug.Log(uid);

            FirebaseDBManager.Instance.GetUserPropertiesByUid(uid, userProperties =>
            {
                Debug.Log("Loaded User Data for UID: " + uid);
                Debug.Log("Current Gold: " + userProperties.gold);

                if (userProperties.gold >= _itemPrice)
                {
                    Inventory inventory = FirebaseDBManager.Instance.GetInventoryByType(inventoryType);

                    if (inventory != null)
                    {
                        inventory.AddItem(_itemName, 1); // Try adding the item directly

                        if (inventory.CanAddItem(_itemName, 1)) // Check if it was possible to add
                        {
                            Debug.Log("Purchase successful");
                            int newGold = userProperties.gold - _itemPrice;
                            FirebaseDBManager.Instance.UpdateGold(uid, newGold);

                            // Update the inventory in database after adding the item
                            FirebaseDBManager.Instance.AddItemToInventory(_itemName, 1, inventoryType);

                            // If using MainThreadDispatcher to handle Unity's main thread operations
                            MainThreadDispatcher.Enqueue(() =>
                            {
                                InventoryManager.Instance.AddItemToGameInventory(_itemName, 1, inventoryType);
                            });
                        }
                        else
                        {
                            Debug.Log("Not enough inventory space");
                        }
                    }
                    else
                    {
                        Debug.Log("Inventory not found");
                    }
                }
                else
                {
                    Debug.Log("Not enough gold to purchase");
                }
            });
        }
    }
}

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
                    FirebaseDBManager.Instance.AddItemToInventory(_itemName, 1, inventoryType, success =>
                    {
                        if (success)
                        {
                            Debug.Log("Purchase successful");
                            int newGold = userProperties.gold - _itemPrice;
                            FirebaseDBManager.Instance.UpdateGold(uid, newGold);

                            MainThreadDispatcher.Enqueue(() =>
                            {
                                InventoryManager.Instance.AddItemToGameInventory(_itemName, 1, inventoryType);
                            });
                        }
                        else
                        {
                            Debug.Log("Not enough inventory space");
                        }
                    });
                }
                else
                {
                    Debug.Log("Not enough gold to purchase");
                }
            });
        }


    }
}

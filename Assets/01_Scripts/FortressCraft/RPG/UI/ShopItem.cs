using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class ShopItem : MonoBehaviour
    {
        public string _itemName;
        public int _itemPrice;
        public Button _purchaseButton;

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
                    MainThreadDispatcher.Enqueue(() =>
                    {
                    });
                }
                else
                {
                    Debug.Log("Not enough inventory space");
                }
            });
        }
    }
}


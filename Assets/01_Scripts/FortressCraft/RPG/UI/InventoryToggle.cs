using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Agit.FortressCraft
{
    public class InventoryToggle : MonoBehaviour
    {
        public GameObject _equipmentInventoryPanel;
        public GameObject _consumableInventoryPanel;
        public GameObject _miscInventoryPanel;

        public Button _equipmentButton;
        public Button _consumableButton;
        public Button _miscButton;


        void Start()
        {
            _equipmentInventoryPanel.SetActive(true);
            _consumableInventoryPanel.SetActive(false);
            _miscInventoryPanel.SetActive(false);

            // Subscribe to button onClick events
            _equipmentButton.onClick.AddListener(() => ToggleInventory("equipment"));
            _consumableButton.onClick.AddListener(() => ToggleInventory("consumable"));
            _miscButton.onClick.AddListener(() => ToggleInventory("misc"));
        }

        void ToggleInventory(string inventoryType)
        {
            // Toggle visibility based on the type of inventory
            _equipmentInventoryPanel.SetActive(inventoryType == "equipment");
            _consumableInventoryPanel.SetActive(inventoryType == "consumable");
            _miscInventoryPanel.SetActive(inventoryType == "misc");

            Debug.Log($"Activated {inventoryType} inventory panel");

        }
    }
}

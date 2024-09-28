using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Required for drag and drop events

public class InventoryItem : MonoBehaviour
{
    public Image _image;
    public Text _nameText;

    private Vector3 originalPosition;
    private Transform originalParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = this.transform.position;
        originalParent = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);  // Move up in the hierarchy to be on top of other UI elements
        GetComponent<CanvasGroup>().blocksRaycasts = false;  // Allow events to pass through the item being dragged
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.position = originalPosition;
        this.transform.SetParent(originalParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (!RectTransformUtility.RectangleContainsScreenPoint((RectTransform)originalParent, eventData.position, null))
        {
            Destroy(this.gameObject);  // Destroy item if it's dropped outside the original parent bounds
        }
    }
}

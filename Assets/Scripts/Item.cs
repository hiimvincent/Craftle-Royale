using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public ItemData itemData;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;

    public void InitializeItem(ItemData newItemData)
    {
        itemData = newItemData;
        image.sprite = newItemData.image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        if (count == 1)
        {
            countText.text = "";
            return;
        }

        countText.text = count.ToString();
    }

    public void Stack(Item other) 
    {
        if (itemData.type == ItemData.ItemType.NonStackable || other.count <= 0)
            return;

        if (count + other.count > itemData.stackableLimit)
        {
            count = itemData.stackableLimit;
            RefreshCount();
            other.count = count + other.count - itemData.stackableLimit;
            other.RefreshCount();
            return;
        }

        count += other.count;
        RefreshCount();
        Destroy(other.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        countText.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}

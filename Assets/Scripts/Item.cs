using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
        image.raycastTarget = false;
        countText.raycastTarget = false;
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

    public void SetCount(int newCount)
    {
        if (newCount < 1) return;

        count = newCount;
        RefreshCount();
    }

    public void Stack(Item other) 
    {
        if (itemData.type == ItemData.ItemType.NonStackable || 
            count >= itemData.stackableLimit ||
            other.count <= 0)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("item left click");
        else if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("item right click");
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int metadata;

    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public ItemData itemData;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;

    public void InitializeItem(ItemData newItemData, int metadata = 0)
    {
        itemData = newItemData;
        this.metadata = metadata;
        image.sprite = newItemData.images[metadata];
        RefreshCount();
        image.raycastTarget = false;
        countText.raycastTarget = false;
    }

    public bool isSame(Item other)
    {
        return itemData.id == other.itemData.id && metadata == other.metadata;
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

    public bool Stack(Item other, bool fromCanvas = false) 
    {
        if (!isSame(other)) return false;

        if (itemData.type == ItemData.ItemType.NonStackable || 
            count >= itemData.stackableLimit ||
            other.count <= 0)
            return false;

        if (count + other.count > itemData.stackableLimit)
        {
            count = itemData.stackableLimit;
            RefreshCount();
            other.count = count + other.count - itemData.stackableLimit;
            other.RefreshCount();
            return false;
        }

        count += other.count;
        RefreshCount();

        if (!fromCanvas)
        {
            Cell par = other.GetComponentInParent<Cell>();
            par.RemoveItem();
        }

        Destroy(other.gameObject);
        return true;
    }
}

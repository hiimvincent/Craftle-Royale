using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCell : MonoBehaviour
{
    public Item item;

    private Item CreateItem(ItemData data, int metadata, int count)
    {
        GameObject newItemGO = Instantiate(GameManager.GameManagerInstance.itemPrefab, transform);
        Item newItem = newItemGO.GetComponent<Item>();
        newItem.InitializeItem(data, metadata);
        newItem.SetCount(count);
        return newItem;
    }

    private void AddItem(Item addedItem)
    {
        addedItem.transform.SetParent(transform);
        addedItem.transform.SetAsLastSibling();
        item = addedItem;
    }

    public void RemoveItem()
    {
        transform.DetachChildren();
        item = null;
    }
    public void DestroyItem()
    {
        if (item == null) return;

        Destroy(item.gameObject);
        item = null;
    }

    public bool SpawnItem(ItemData data, int metadata, int count)
    {
        if (item != null) return false;

        AddItem(CreateItem(data, metadata, count));
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingResultCell : MonoBehaviour
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

    public bool SpawnItem(ItemData data, int metadata, int count)
    {
        if (item != null) return false;

        AddItem(CreateItem(data, metadata, count));
        return true;
    }

    public void OnNewResult(Result result)
    {
        if (result == null)
        {
            RemoveItem();
            return;
        }

        GameManager gm = GameManager.GameManagerInstance;
        ItemData itemData = gm.itemDataManager.getItemDataById(result.id);
        SpawnItem(itemData, result.metadata, result.quantity);
    }
}

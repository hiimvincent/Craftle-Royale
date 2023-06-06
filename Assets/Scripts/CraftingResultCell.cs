using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingResultCell : MonoBehaviour, IPointerClickHandler, IDragHandler
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

    public void OnNewResult(Result result)
    {
        DestroyItem();

        if (result == null) return;
        
        GameManager gm = GameManager.GameManagerInstance;
        ItemData itemData = gm.itemDataManager.getItemDataById(result.id);
        SpawnItem(itemData, result.metadata, result.quantity);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;

        if (eventData.button == PointerEventData.InputButton.Middle) return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            OnShiftClick(eventData);
            return;
        }

        GameManager gm = GameManager.GameManagerInstance;

        if (gm.curItem == null)
        {
            gm.curItem = item;
            RemoveItem();
            gm.curItem.transform.SetParent(gm.canvas.transform);
            gm.cm.DecrementCraftingBoard();
            return;
        }

        if (!gm.curItem.IsSame(item)) return;

        if (item.itemData.type == ItemData.ItemType.NonStackable) return;

        if (gm.curItem.count + item.count > gm.curItem.itemData.stackableLimit) return;

        Item prev = item;
        RemoveItem();
        gm.curItem.Stack(prev);
        gm.cm.DecrementCraftingBoard();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle) return;
    }

    public void OnShiftClick(PointerEventData eventData)
    {
        GameManager gm = GameManager.GameManagerInstance;
        int maxPossibleTimes = gm.cm.GetNonZeroMin();
        int curCount = item.count;

        int timesPlaced = PlaceMax();

        if (item != null && timesPlaced == maxPossibleTimes)
        {
            DestroyItem();
        }

        gm.cm.DecrementByAmount(timesPlaced);
    }

    private int PlaceMax()
    {
        GameManager gm = GameManager.GameManagerInstance;
        int maxPossibleTimes = gm.cm.GetNonZeroMin();
        int placeableRem = maxPossibleTimes * item.count;
        int curUnitRem = item.count;
        List<Vector3Int> pending = new List<Vector3Int>();

        Row[] invRows = gm.im.rows;

        if (item.itemData.type == ItemData.ItemType.Stackable)
        {
            for (int rIndex = 0; rIndex < invRows.Length; rIndex++)
            {
                for (int cIndex = 0; cIndex < invRows[rIndex].cells.Length; cIndex++)
                {
                    Cell cur = invRows[rIndex].cells[cIndex];

                    if (cur.item == null || !cur.item.IsSame(item)) continue;

                    if (cur.item.count == cur.item.itemData.stackableLimit) continue;

                    if (cur.item.count + curUnitRem > cur.item.itemData.stackableLimit)
                    {
                        int dif = cur.item.itemData.stackableLimit - cur.item.count;
                        pending.Add(new Vector3Int(rIndex, cIndex, dif));
                        curUnitRem -= dif;
                        continue;
                    }

                    if (curUnitRem != item.count)
                    {
                        ProcessPending(invRows, pending);
                        cur.item.AddCount(curUnitRem);
                        placeableRem -= item.count;
                        curUnitRem = item.count;
                    }

                    if (placeableRem == 0) return maxPossibleTimes;

                    int dif2 = cur.item.itemData.stackableLimit - cur.item.count;

                    if (dif2 >= placeableRem)
                    {
                        cur.item.AddCount(placeableRem);
                        return maxPossibleTimes;
                    }

                    if (dif2 >= item.count)
                    {
                        int numPlaces = dif2 / item.count;
                        cur.item.AddCount(numPlaces * item.count);
                        placeableRem -= numPlaces * item.count;
                    }

                    int dif3 = cur.item.itemData.stackableLimit - cur.item.count;

                    if (dif3 != 0)
                    {
                        pending.Add(new Vector3Int(rIndex, cIndex, dif3));
                        curUnitRem -= dif3;
                    }
                }
            }
        }

        for (int rIndex = 0; rIndex < invRows.Length; rIndex++)
        {
            for (int cIndex = 0; cIndex < invRows[rIndex].cells.Length; cIndex++)
            {
                Cell cur = invRows[rIndex].cells[cIndex];

                if (cur.item != null) continue;

                if (item.itemData.type == ItemData.ItemType.NonStackable)
                {
                    if (placeableRem == 1)
                    {
                        Item prev = item;
                        RemoveItem();
                        cur.AddItem(prev);
                        return maxPossibleTimes;
                    }
                    else
                    {
                        cur.SpawnItem(item.itemData, item.metadata, cur.transform);
                        cur.item.SetCount(1);
                        placeableRem -= 1;
                        continue;
                    }
                }

                if (curUnitRem != item.count)
                {
                    ProcessPending(invRows, pending);
                    cur.SpawnItem(item.itemData, item.metadata, cur.transform);
                    cur.item.SetCount(curUnitRem);
                    placeableRem -= item.count;
                    curUnitRem = item.count;
                }

                if (placeableRem == 0) return maxPossibleTimes;

                int dif = cur.item == null ? item.itemData.stackableLimit : item.itemData.stackableLimit - cur.item.count;

                if (dif >= placeableRem)
                {
                    if (cur.item != null)
                    {
                        cur.item.AddCount(placeableRem);
                        return maxPossibleTimes;
                    }

                    cur.SpawnItem(item.itemData, item.metadata, cur.transform);
                    cur.item.SetCount(placeableRem);
                    return maxPossibleTimes;
                }

                if (dif >= item.count)
                {
                    int numPlaces = dif / item.count;

                    if (cur.item != null)
                    {
                        cur.item.AddCount(numPlaces * item.count);
                    }
                    else
                    {
                        cur.SpawnItem(item.itemData, item.metadata, cur.transform);
                        cur.item.SetCount(numPlaces * item.count);
                    }

                    placeableRem -= numPlaces * item.count;
                }

                int dif2 = cur.item.itemData.stackableLimit - cur.item.count;

                if (dif2 != 0)
                {
                    pending.Add(new Vector3Int(rIndex, cIndex, dif2));
                    curUnitRem -= dif2;
                }
            }
        }

        return (maxPossibleTimes * item.count - placeableRem) / item.count;
    }

    private void ProcessPending(Row[] invRows, List<Vector3Int> pending)
    {
        foreach(Vector3Int cur in pending)
        {
            invRows[cur.x].cells[cur.y].item.AddCount(cur.z);
        }

        pending.Clear();
    }
}

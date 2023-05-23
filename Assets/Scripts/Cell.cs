using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public bool isInventory = true;
    public Item item;
    public Vector2 pos;

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

    private Item CreateItem(ItemData data, Transform t)
    {
        GameObject newItemGO = Instantiate(GameManager.GameManagerInstance.itemPrefab, t);
        Item newItem = newItemGO.GetComponent<Item>();
        newItem.InitializeItem(data);
        return newItem;
    }

    public bool SpawnItem(ItemData data, Transform t)
    {
        if (item != null) return false;

        AddItem(CreateItem(data, t));
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle) return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            OnShiftClick(eventData);
            return;
        }

        GameManager gm = GameManager.GameManagerInstance;

        Item cellCur = item;
        Item oldCur = gm.curItem;

        if (oldCur == null && cellCur == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (cellCur == null)
            {
                AddItem(gm.curItem);
                gm.curItem = null;
                return;
            }

            if (gm.curItem != null && cellCur.itemData.id == oldCur.itemData.id)
            {
                if (cellCur.Stack(gm.curItem, fromCanvas: true))
                    gm.curItem = null;
                return;
            }

            gm.curItem = cellCur;
            RemoveItem();
            gm.curItem.transform.SetParent(gm.canvas.transform);

            if (oldCur != null)
            {
                AddItem(oldCur);
            }

            return;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cellCur == null)
            {
                if (oldCur.count == 1)
                {
                    AddItem(gm.curItem);
                    gm.curItem = null;
                    return;
                }
                else
                {
                    SpawnItem(gm.curItem.itemData, transform);
                    item.SetCount(1);
                    gm.curItem.SetCount(gm.curItem.count - 1);
                    return;
                }
            }

            if (oldCur == null)
            {
                if (cellCur.count == 1)
                {
                    gm.curItem = cellCur;
                    RemoveItem();
                    gm.curItem.transform.SetParent(gm.canvas.transform);
                    return;
                }
                else
                {
                    int half = (int)Mathf.Ceil(cellCur.count / 2.0f);
                    Item newItem = CreateItem(cellCur.itemData, gm.canvas.transform);
                    newItem.SetCount(half);
                    gm.curItem = newItem;
                    cellCur.SetCount(cellCur.count - half);
                    return;
                }
            }

            if (cellCur.itemData.id == gm.curItem.itemData.id)
            {
                if (cellCur.count >= cellCur.itemData.stackableLimit) return;

                if (gm.curItem.count == 1)
                {
                    if (cellCur.Stack(gm.curItem, fromCanvas: true))
                        gm.curItem = null;
                    return;
                }
                else
                {
                    cellCur.SetCount(cellCur.count + 1);
                    gm.curItem.SetCount(gm.curItem.count - 1);
                    return;
                }
            }

            gm.curItem = cellCur;
            RemoveItem();
            gm.curItem.transform.SetParent(gm.canvas.transform);
            AddItem(oldCur);

            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.GameManagerInstance.curItem != null) return;

        OnPointerClick(eventData);
    }

    public void OnShiftClick(PointerEventData eventData)
    {
        if (item == null) return;

        GameManager gm = GameManager.GameManagerInstance;

        if (isInventory)
        {
            Row[] craftRows = gm.cm.rows;
            if (AttemptPlaceItem(craftRows)) return;
        }

        Row[] invRows = gm.im.rows;
        AttemptPlaceItem(invRows);
    }

    public bool AttemptPlaceItem(Row[] rows)
    {
        Item cellCur = item;

        if (AttemptStackItem(rows)) return true;

        for (int rIndex = 0; rIndex < rows.Length; rIndex++)
        {
            for (int cIndex = 0; cIndex < rows[rIndex].cells.Length; cIndex++)
            {
                Cell cur = rows[rIndex].cells[cIndex];

                if (rIndex == pos.x && cIndex == pos.y && isInventory == cur.isInventory)
                    return true;

                if (cur.item == null)
                {
                    RemoveItem();
                    cur.AddItem(cellCur);
                    return true;
                }
            }
        }

        return false;
    }

    private bool AttemptStackItem(Row[] rows)
    {
        Item cellCur = item;

        for (int rIndex = 0; rIndex < rows.Length; rIndex++)
        {
            for (int cIndex = 0; cIndex < rows[rIndex].cells.Length; cIndex++)
            {
                Cell cur = rows[rIndex].cells[cIndex];

                if (rIndex == pos.x && cIndex == pos.y && isInventory == cur.isInventory)
                    return false;

                if (cur.item == null || cur.item.itemData.id != cellCur.itemData.id) continue;

                if (cellCur.itemData.type == ItemData.ItemType.NonStackable ||
                    cur.item.count == cur.item.itemData.stackableLimit) continue;

                if (cur.item.Stack(cellCur)) return true;
            }
        }

        return false;
    }
}

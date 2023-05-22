using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public GameManager gameManager;
    public bool isInventory = true;
    public Item item;

    private void AddItem(Item addedItem)
    {
        addedItem.transform.SetParent(transform);
        addedItem.transform.SetAsLastSibling();
        item = addedItem;
    }

    private void RemoveItem()
    {
        transform.DetachChildren();
        item = null;
    }

    private Item CreateItem(ItemData data, Transform t)
    {
        GameObject newItemGO = Instantiate(gameManager.itemPrefab, t);
        Item newItem = newItemGO.GetComponent<Item>();
        newItem.InitializeItem(data);
        return newItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameManager = GameManager.GameManagerInstance;

        Item cellCur = GetComponentInChildren<Item>();
        Item oldCur = gameManager.curItem;

        if (oldCur == null && cellCur == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (cellCur == null)
            {
                AddItem(gameManager.curItem);
                gameManager.curItem = null;
                return;
            }

            if (gameManager.curItem != null && cellCur.itemData.id == oldCur.itemData.id)
            {
                cellCur.Stack(gameManager.curItem);
                return;
            }

            gameManager.curItem = cellCur;
            RemoveItem();
            gameManager.curItem.transform.SetParent(gameManager.canvas.transform);

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
                    AddItem(gameManager.curItem);
                    gameManager.curItem = null;
                    return;
                }
                else
                {
                    Item newItem = CreateItem(gameManager.curItem.itemData, transform);
                    newItem.SetCount(1);
                    gameManager.curItem.SetCount(gameManager.curItem.count - 1);
                    return;
                }
            }

            if (oldCur == null)
            {
                if (cellCur.count == 1)
                {
                    gameManager.curItem = cellCur;
                    RemoveItem();
                    gameManager.curItem.transform.SetParent(gameManager.canvas.transform);
                    return;
                }
                else
                {
                    int half = (int)Mathf.Ceil(cellCur.count / 2.0f);
                    Item newItem = CreateItem(cellCur.itemData, gameManager.canvas.transform);
                    newItem.SetCount(half);
                    gameManager.curItem = newItem;
                    cellCur.SetCount(cellCur.count - half);
                    return;
                }
            }

            if (cellCur.itemData.id == gameManager.curItem.itemData.id)
            {
                if (cellCur.count >= cellCur.itemData.stackableLimit) return;

                if (gameManager.curItem.count == 1)
                {
                    cellCur.Stack(gameManager.curItem);
                    return;
                }
                else
                {
                    cellCur.SetCount(cellCur.count + 1);
                    gameManager.curItem.SetCount(gameManager.curItem.count - 1);
                    return;
                }
            }

            gameManager.curItem = cellCur;
            RemoveItem();
            gameManager.curItem.transform.SetParent(gameManager.canvas.transform);
            AddItem(oldCur);

            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.GameManagerInstance.curItem != null) return;

        OnPointerClick(eventData);
    }
}

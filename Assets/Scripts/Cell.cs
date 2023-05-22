using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public GameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Item item = dropped.GetComponent<Item>();

        if (transform.childCount == 0)
        {
            item.parentAfterDrag = transform;
            return;
        }

        Item cur = GetComponentInChildren<Item>();
        if (cur != null && cur.itemData.id == item.itemData.id)
        {
            cur.Stack(item);
        }
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
                gameManager.curItem.transform.SetParent(transform);
                gameManager.curItem.transform.SetAsLastSibling();
                gameManager.curItem = null;
                return;
            }

            if (gameManager.curItem != null && cellCur.itemData.id == oldCur.itemData.id)
            {
                cellCur.Stack(gameManager.curItem);
                return;
            }

            gameManager.curItem = cellCur;
            transform.DetachChildren();
            gameManager.curItem.transform.SetParent(gameManager.canvas.transform);

            if (oldCur != null)
            {
                oldCur.transform.SetParent(transform);
                oldCur.transform.SetAsLastSibling();
            }

            return;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cellCur == null)
            {
                if (oldCur.count == 1)
                {
                    gameManager.curItem.transform.SetParent(transform);
                    gameManager.curItem.transform.SetAsLastSibling();
                    gameManager.curItem = null;
                    return;
                }
                else
                {
                    GameObject newItemGO = Instantiate(gameManager.curItem.gameObject, transform);
                    Item newItem = newItemGO.GetComponent<Item>();
                    newItem.InitializeItem(gameManager.curItem.itemData);
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
                    transform.DetachChildren();
                    gameManager.curItem.transform.SetParent(gameManager.canvas.transform);
                    return;
                }
                else
                {
                    int half = (int)Mathf.Ceil(cellCur.count / 2.0f);

                    GameObject newItemGO = Instantiate(gameManager.itemPrefab, gameManager.canvas.transform);
                    Item newItem = newItemGO.GetComponent<Item>();

                    newItem.InitializeItem(cellCur.itemData);
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
            transform.DetachChildren();
            gameManager.curItem.transform.SetParent(gameManager.canvas.transform);

            oldCur.transform.SetParent(transform);
            oldCur.transform.SetAsLastSibling();

            return;
        }
    }
}

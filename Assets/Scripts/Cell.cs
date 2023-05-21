using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IDropHandler
{
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

}

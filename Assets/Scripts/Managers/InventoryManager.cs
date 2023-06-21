using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Row[] rows;
    public int[] testingItems = new int[] {4, 264, 265};

    List<Result> originalInvCopy;
    int[] originalIndicesCopy;


    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    IEnumerator Start()
    {
        GameManager gm = GameManager.GameManagerInstance;
        yield return new WaitUntil(() => gm.IsInitialized);

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].pos = new Vector2(i, j);
                //rows[i].cells[j].SpawnItem(gm.itemDataManager.GetItemDataById(testingItems[i]), 0, rows[i].cells[j].transform);
                //rows[i].cells[j].item.SetCount(10);
            }
        }

    }

    public void SetInventory(List<Result> inv, int[] indices, bool isReset = false)
    {
        GameManager gm = GameManager.GameManagerInstance;

        if (!isReset)
        {
            originalInvCopy = InvDeepCopy(inv);
            originalIndicesCopy = indices;
        }

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].DestroyItem();
            }
        }

        for (int curI = 0; curI < inv.Count; curI++)
        {
            int curPos = indices[curI];
            int i = curPos / rows[0].cells.Length;
            int j = curPos % rows[0].cells.Length;

            rows[i].cells[j].SpawnItem(gm.itemDataManager.GetItemDataById(inv[curI].id), inv[curI].metadata, rows[i].cells[j].transform);
            rows[i].cells[j].item.SetCount(inv[curI].quantity);
        }
    }

    public void ResetInventory()
    {
        SetInventory(originalInvCopy, originalIndicesCopy, true);
    }

    private List<Result> InvDeepCopy(List<Result> inv)
    {
        if (inv == null) return null;

        List<Result> res = new List<Result>();

        foreach (Result cur in inv)
        {
            res.Add(new Result(cur.id, cur.metadata, cur.quantity));
        }

        return res;
    }

    public bool DetectWin(Result target)
    {
        GameManager gm = GameManager.GameManagerInstance;

        int targetAmt = 0;

        if (gm.curItem != null && gm.curItem.itemData.id == target.id && gm.curItem.metadata == target.metadata)
        {
            targetAmt += gm.curItem.count;

            if (targetAmt >= target.quantity) return true;
        }

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                Item cur = rows[i].cells[j].item;

                if (cur == null) continue;

                if (cur.itemData.id == target.id && cur.metadata == target.metadata)
                {
                    targetAmt += cur.count;

                    if (targetAmt >= target.quantity) return true;
                }
            }
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Row[] rows;
    public int[] testingItems = new int[] {4, 264, 265};

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
                rows[i].cells[j].SpawnItem(gm.itemDataManager.GetItemDataById(testingItems[i]), 0, rows[i].cells[j].transform);
                rows[i].cells[j].item.SetCount(10);
            }
        }

        gm.roundManager.OnNewRound();
    }

    public void SetInventory(List<Result> inv, int[] indices)
    {
        GameManager gm = GameManager.GameManagerInstance;

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
}

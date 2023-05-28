using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Row[] rows;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    IEnumerator Start()
    {
        GameManager gm = GameManager.GameManagerInstance;
        yield return new WaitUntil(() => gm.IsInitialized);

        for (int i = 1; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].pos = new Vector2(i, j);
                rows[i].cells[j].SpawnItem(gm.itemDataManager.getItemDataById(280), 0, rows[i].cells[j].transform);
                rows[i].cells[j].item.SetCount(10);
            }
        }
    }
}

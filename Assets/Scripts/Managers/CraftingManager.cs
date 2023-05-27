using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public CraftingResultCell craftingCellResult;
    public Row[] rows;

    string lastGridString;

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
                rows[i].cells[j].SpawnItem(gm.itemDataManager.getItemDataById(i+3), 0, rows[i].cells[j].transform);
            }
        }
    }

    public string getGridString()
    {
        string res = "";

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].cells.Length; col++)
            {
                Cell curCell = rows[row].cells[col];
                if (curCell.item == null)
                {
                    res += "0000";
                }
                else
                {
                    res += curCell.item.itemData.id.ToString("000") + curCell.item.metadata.ToString();
                }
            }
        }

        lastGridString = res;

        return res;
    }

    public void OnGridChange()
    {
        string prev = lastGridString;
        if (getGridString() == prev) return;

        GameManager gm = GameManager.GameManagerInstance;
        craftingCellResult.OnNewResult(gm.recipeManager.FindMatch(lastGridString));
    }
}

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
                rows[i].cells[j].item.SetCount(10);
            }
        }
    }

    public string getGridString(BoundingBox dim)
    {
        string res = "";

        for (int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for (int col = dim.topLeftY; col <= dim.bottomRightY; col++)
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

    public BoundingBox GetBoundingBox()
    {
        int xMin = -1, xMax = -1, yMin = -1, yMax = -1;

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].cells.Length; col++)
            {
                Cell curCell = rows[row].cells[col];

                if (curCell.item == null) continue;

                xMin = (xMin == -1) ? row : Mathf.Min(xMin, row);
                yMin = (yMin == -1) ? col : Mathf.Min(yMin, col);
                xMax = (xMax == -1) ? row : Mathf.Max(xMax, row);          
                yMax = (yMax == -1) ? col : Mathf.Max(yMax, col);
            }
        }

        if (xMin == -1) return null;

        return new BoundingBox(xMin, yMin, xMax, yMax);
    }

    public void OnGridChange()
    {
        string prev = lastGridString;
        BoundingBox curDim = GetBoundingBox(); 
        if (getGridString(curDim) == prev) return;

        GameManager gm = GameManager.GameManagerInstance;
        craftingCellResult.OnNewResult(gm.recipeManager.FindMatch(lastGridString, curDim));
    }

    public void OnDecrement()
    {
        GameManager gm = GameManager.GameManagerInstance;
        BoundingBox curDim = GetBoundingBox();
        craftingCellResult.OnNewResult(gm.recipeManager.FindMatch(getGridString(curDim), curDim));
    }

    public void DecrementCraftingBoard()
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].cells.Length; col++)
            {
                Cell curCell = rows[row].cells[col];

                if (curCell.item == null) continue;

                if (curCell.item.count == 1)
                {
                    curCell.DestroyItem();
                }
                else
                {
                    curCell.item.AddCount(-1);
                }
            }
        }

        OnDecrement();
    }

    public int GetNonZeroMin()
    {
        int curMin = -1;

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].cells.Length; col++)
            {
                Cell curCell = rows[row].cells[col];

                if (curCell.item == null) continue;

                if (curMin == -1)
                {
                    curMin = curCell.item.count;
                }
                else
                {
                    curMin = Mathf.Min(curMin, curCell.item.count);
                }
            }
        }

        return curMin;
    }

    public void DecrementByAmount(int amount)
    {
        if (amount < 1) return;

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].cells.Length; col++)
            {
                Cell curCell = rows[row].cells[col];

                if (curCell.item == null) continue;

                if (curCell.item.count <= amount)
                {
                    curCell.DestroyItem();
                }
                else
                {
                    curCell.item.AddCount(-amount);
                }
            }
        }

        OnDecrement();
    }
}

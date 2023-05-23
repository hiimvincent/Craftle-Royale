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

    void Start()
    {
        GameManager gm = GameManager.GameManagerInstance;
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].pos = new Vector2(i, j);
                rows[i].cells[j].SpawnItem(gm.datas[i], rows[i].cells[j].transform);
            }
        }
    }
}

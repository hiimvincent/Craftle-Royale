using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public Row[] rows;
    //replace with gamemanager reference
    public GameObject itemPrefab;
    public ItemData[] datas;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    void Start()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                SpawnNewItem(datas[i], rows[i].cells[j]);
            }
        }

    }

    void SpawnNewItem(ItemData itemData, Cell cell)
    {
        GameObject newItemGO = Instantiate(itemPrefab, cell.transform);
        Item newItem = newItemGO.GetComponent<Item>();
        newItem.InitializeItem(itemData);
    }

}

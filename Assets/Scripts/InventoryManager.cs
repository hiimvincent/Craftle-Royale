using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Row[] rows;
    public GameObject ItemPrefab;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    void Start()
    {

    }
}

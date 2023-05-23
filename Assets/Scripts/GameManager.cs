using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance
    {
        get; private set;
    }

    public Canvas canvas;
    public InventoryManager im;
    public CraftingManager cm;
    public GameObject itemPrefab;
    public ItemData[] datas;

    public Item curItem = null;

    private void Awake()
    {
        if (GameManagerInstance == null)
        {
            GameManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (curItem == null) return;

        curItem.transform.position = Input.mousePosition;
    }
}

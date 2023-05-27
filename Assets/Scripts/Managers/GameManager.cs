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
    public ItemDataManager itemDataManager;
    public RecipeManager recipeManager;

    public Item curItem = null;

    public bool IsInitialized { get; private set; }

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

    private IEnumerator Start()
    {
        itemDataManager = GetComponent<ItemDataManager>();
        recipeManager = GetComponent<RecipeManager>();
        yield return new WaitUntil(() => itemDataManager.IsInitialized && recipeManager.IsInitialized);

        IsInitialized = true;
    }

    private void Update()
    {
        if (curItem == null) return;

        curItem.transform.position = Input.mousePosition;
    }
}

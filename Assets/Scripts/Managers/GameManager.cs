using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance
    {
        get; private set;
    }

    public GameObject itemPrefab;
    public Canvas canvas;

    public InventoryManager invManager;
    public CraftingManager craftManager;
    public ItemDataManager itemDataManager;
    public RecipeManager recipeManager;
    public RoundManager roundManager;

    public Item curItem = null;
    public Result curTarget;

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
        roundManager = GetComponent<RoundManager>();

        yield return new WaitUntil(() => itemDataManager.IsInitialized && recipeManager.IsInitialized && roundManager.IsInitialized);

        IsInitialized = true;
    }

    private void Update()
    {
        if (curItem == null) return;

        curItem.transform.position = Input.mousePosition;
    }
}

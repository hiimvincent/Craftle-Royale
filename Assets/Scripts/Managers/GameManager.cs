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
    public GameObject HomeScreen;
    public Canvas canvas;

    public InventoryManager invManager;
    public CraftingManager craftManager;
    public ItemDataManager itemDataManager;
    public RecipeManager recipeManager;
    public RoundManager roundManager;

    public Item curItem = null;
    public Result curTarget;
    public int curStreak;
    public int maxStreak;

    public bool IsInitialized { get; private set; }
    public bool WinState { get; set; }

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

    public void DestroyCurItem()
    {
        if (curItem == null) return;

        Destroy(curItem);
        curItem = null;
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
        if (curItem == null || WinState) return;

        curItem.transform.position = Input.mousePosition;
    }

    public void OnNewGame()
    {
        roundManager.OnNewRound();
        HomeScreen.SetActive(false);
    }

    public void OnGotoHome()
    {
        HomeScreen.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

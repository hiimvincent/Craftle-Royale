using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] TextAsset roundDataJson;
    [SerializeField] TargetCell targetCell;
    [SerializeField] GameObject victoryPopup;
    [SerializeField] TextMeshProUGUI curStreakText;
    [SerializeField] TextMeshProUGUI maxStreakText;

    RoundData rd;
    Dictionary<string, int> curInventory;
    List<Result> formattedCurInventory;

    public bool IsInitialized { get; private set; }

    const int INV_SIZE = 27;
    const int TARGET_AMT_LIMIT = 6;
    const int REV_CRAFT_LIMIT = 4;
    const int MAX_FULL_CELLS = 18;
    const int BREAK_DOWN_THRESHOLD = 80;
    const int SPLIT_MIN = 4;
    const int SPLIT_THRESHOLD = 30;
    const int U_ITEM_STACK_MIN = 2;
    const int U_ITEM_STACK_MAX = 8;


    private void Awake()
    {
        string jsonString = roundDataJson.ToString();
        rd = JsonUtility.FromJson<RoundData>(jsonString);

        IsInitialized = true;
    }

    private void Start()
    {
        DisableVictoryPopup();
        InitializeStreaks();
    }

    public void OnNewRound()
    {
        GameManager gm = GameManager.GameManagerInstance;

        ItemPoolItem target = SelectNewTarget();
        int targetAmt = Random.Range(1, TARGET_AMT_LIMIT);
        curInventory = new Dictionary<string, int>();
        formattedCurInventory = new List<Result>();
        curInventory[target.GetResString()] = targetAmt;
        gm.curTarget = new Result(target.id, target.metadata, targetAmt);
        int revCraftingAmt = Random.Range(2, REV_CRAFT_LIMIT);

        ReverseCraftInventory(forceRev:true);

        for (int i = 0; i < revCraftingAmt - 1; i++)
        {
            ReverseCraftInventory();
        }

        FormatInventory();
        SplitItemStacks();
        AddUnhelfulItems();
        DisableVictoryPopup();

        targetCell.DestroyItem();
        gm.DestroyCurItem();
        gm.craftManager.ClearCraftingBoard();
        gm.WinState = false;

        gm.invManager.SetInventory(formattedCurInventory, GetShuffledIndices());
        targetCell.SpawnItem(gm.itemDataManager.GetItemDataById(target.id), target.metadata, targetAmt);
    }

    public void ProcessNewCraft()
    {
        GameManager gm = GameManager.GameManagerInstance;
        if(gm.invManager.DetectWin(gm.curTarget))
        {
            IncrementStreak();
            EnableVictoryPopup();
            gm.WinState = true;
        }
    }

    public void EnableVictoryPopup()
    {
        victoryPopup.SetActive(true);
    }

    public void DisableVictoryPopup()
    {
        victoryPopup.SetActive(false);
    }

    private ItemPoolItem SelectNewTarget()
    {
        GameManager gm = GameManager.GameManagerInstance;
        ItemPoolItem res = rd.itemPool[Random.Range(0, rd.itemPool.Count)];

        while (gm.recipeManager.GetIngredients(res.GetResString()) == null) {
            res = rd.itemPool[Random.Range(0, rd.itemPool.Count)];
        }

        return res;
    }

    private void ReverseCraftInventory(bool forceRev = false)
    {
        int fullSlots = GetUsedInvSlots(curInventory);

        if (fullSlots >= MAX_FULL_CELLS) return;

        bool isFull = false;

        GameManager gm = GameManager.GameManagerInstance;
        Dictionary<string, int> newInventory = new Dictionary<string, int>();
        ItemIngredientTable itemRevRecipes;

        foreach (var item in curInventory)
        {
            if (isFull)
            {
                AddToInv(newInventory, item.Key, item.Value);
                continue;
            }

            int newSlots = GetUsedInvSlots(newInventory);

            if (newSlots + fullSlots - 1 > MAX_FULL_CELLS)
            {
                isFull = true;
            }

            itemRevRecipes = gm.recipeManager.GetIngredients(item.Key);

            if (itemRevRecipes == null || (!forceRev && Random.Range(0, 100) > BREAK_DOWN_THRESHOLD) || isFull)
            {
                AddToInv(newInventory, item.Key, item.Value);
                continue;
            }
            
            Dictionary<string, int> randRevRecipe = itemRevRecipes.table[Random.Range(0, itemRevRecipes.table.Count)];
            int revTimes = forceRev ? item.Value : Random.Range(1, item.Value + 1);

            int remAmt = item.Value - revTimes;
            AddRecipeToInv(newInventory, randRevRecipe, revTimes);
            
            if (remAmt > 0)
            {
                AddToInv(newInventory, item.Key, remAmt);
            }
        }

        curInventory = newInventory;
    }

    private void AddUnhelfulItems()
    {
        if (formattedCurInventory == null || formattedCurInventory.Count >= MAX_FULL_CELLS) return;

        GameManager gm = GameManager.GameManagerInstance;

        List<ItemData> randomItemList = gm.itemDataManager.GetRandomItemDatas(MAX_FULL_CELLS);

        foreach (ItemData curData in randomItemList)
        {
            int randMetadata = Random.Range(0, curData.names.Length);
            string curItemString = curData.id.ToString("D3") + randMetadata.ToString("D2");

            if (curInventory.ContainsKey(curItemString) || (curData.id == gm.curTarget.id && randMetadata == gm.curTarget.metadata)) continue;

            int randAmt = curData.type == ItemData.ItemType.NonStackable ? 1 : Random.Range(U_ITEM_STACK_MIN, U_ITEM_STACK_MAX + 1);
            formattedCurInventory.Add(new Result(curData.id, randMetadata, randAmt));
            AddToInv(curInventory, curItemString, randAmt);

            if (formattedCurInventory.Count >= MAX_FULL_CELLS) return;
        }
    }

    private void AddToInv(Dictionary<string, int> inv, string itemStr, int amt = 1)
    {
        if (inv == null) return;

        if (inv.ContainsKey(itemStr))
        {
            inv[itemStr] += amt;
        }
        else
        {
            inv[itemStr] = amt;
        }
    }

    private void AddRecipeToInv(Dictionary<string, int> inv, Dictionary<string, int> recipe, int times)
    {
        foreach (var item in recipe)
        {
            AddToInv(inv, ReplaceSpecialMetadata(item.Key), item.Value * times);
        }
    }

    private string ReplaceSpecialMetadata(string resString)
    {
        if (resString.Substring(3, 2) != "99") return resString;

        GameManager gm = GameManager.GameManagerInstance;
        string resId = resString.Substring(0, 3);
        ItemData itemData = gm.itemDataManager.GetItemDataById(int.Parse(resId));
        string resMetadata = Random.Range(0, itemData.names.Length).ToString("D2");
        
        return resId + resMetadata;
    }

    private int GetUsedInvSlots(Dictionary<string, int> inv)
    {
        int count = 0;
        GameManager gm = GameManager.GameManagerInstance;
        ItemData itemData;

        foreach (var item in inv)
        {

            itemData = gm.itemDataManager.GetItemDataById(int.Parse(item.Key.Substring(0, 3)));

            if (itemData.type == ItemData.ItemType.NonStackable)
            {
                count += item.Value;
            }
            else
            {
                count += (int)Mathf.Ceil((float)item.Value / itemData.stackableLimit);
            }
        }

        return count;
    }

    private void FormatInventory()
    {
        GameManager gm = GameManager.GameManagerInstance;

        foreach (var item in curInventory)
        {
            ItemData itemData = gm.itemDataManager.GetItemDataById(int.Parse(item.Key.Substring(0, 3)));
            int metadata = int.Parse(item.Key.Substring(3, 2));

            if (itemData.type == ItemData.ItemType.NonStackable)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    formattedCurInventory.Add(new Result(itemData.id, metadata, 1));
                }
            }
            else
            {
                if (item.Value <= itemData.stackableLimit)
                {
                    formattedCurInventory.Add(new Result(itemData.id, metadata, item.Value));
                    continue;
                }

                int remAmt = item.Value;
                int roundAmt;

                while (remAmt > 0)
                {
                    if (remAmt >= itemData.stackableLimit)
                    {
                        roundAmt = itemData.stackableLimit;
                        remAmt -= itemData.stackableLimit;
                    }
                    else
                    {
                        roundAmt = remAmt;
                        remAmt = 0;
                    }

                    formattedCurInventory.Add(new Result(itemData.id, metadata, roundAmt));
                }
            }
        }
    }

    private void SplitItemStacks()
    {
        if (formattedCurInventory.Count >= MAX_FULL_CELLS) return;

        int indexLimit = formattedCurInventory.Count;

        for (int i = 0; i < indexLimit; i++)
        {
            Result cur = formattedCurInventory[i];

            if (cur.quantity < SPLIT_MIN  || Random.Range(0, 100) > SPLIT_THRESHOLD) continue;

            int firstSplitSize = Random.Range(cur.quantity / 4, cur.quantity * 3 / 4);
            formattedCurInventory.Add(new Result(cur.id, cur.metadata, cur.quantity - firstSplitSize));
            cur.quantity = firstSplitSize;

            if (formattedCurInventory.Count >= MAX_FULL_CELLS) return;
        }
    }

    private int[] GetShuffledIndices()
    {
        int[] newIndices = Enumerable.Range(0, INV_SIZE).ToArray();

        for (int i = 0; i < newIndices.Length; i++)
        {
            int temp = newIndices[i];
            int randomIndex = Random.Range(0, newIndices.Length);
            newIndices[i] = newIndices[randomIndex];
            newIndices[randomIndex] = temp;
        }

        return newIndices;
    }
    
    private void InitializeStreaks()
    {
        GameManager gm = GameManager.GameManagerInstance;

        gm.maxStreak = PlayerPrefs.GetInt("maxStreak", 0);
        gm.curStreak = 0;
        UpdateStreakText();
    }

    private void IncrementStreak()
    {
        GameManager gm = GameManager.GameManagerInstance;

        gm.curStreak += 1;

        if (gm.curStreak > gm.maxStreak)
        {
            PlayerPrefs.SetInt("maxStreak", gm.curStreak);
            gm.maxStreak = gm.curStreak;
        }

        UpdateStreakText();
    }

    private void UpdateStreakText()
    {
        GameManager gm = GameManager.GameManagerInstance;

        curStreakText.text = gm.curStreak.ToString("D3");
        maxStreakText.text = gm.maxStreak.ToString("D3");
    }
}

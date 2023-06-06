using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] TextAsset roundDataJson;

    RoundData rd;
    Dictionary<string, int> curInventory;

    const int TARGET_AMT_LIMIT = 6;
    const int REV_CRAFT_LIMIT = 4;
    const int MAX_FULL_CELLS = 20;
    const int BREAK_DOWN_THRESHOLD = 60;


    private void Awake()
    {
        string jsonString = roundDataJson.ToString();
        rd = JsonUtility.FromJson<RoundData>(jsonString);


    }

    public void OnNewRound()
    {
        ItemPoolItem target = SelectNewTarget();
        int targetAmt = Random.Range(1, TARGET_AMT_LIMIT);
        curInventory = new Dictionary<string, int>();
        curInventory[target.GetResString()] = targetAmt;
        int revCraftingAmt = Random.Range(1, REV_CRAFT_LIMIT);

        for (int i = 0; i < revCraftingAmt; i++)
        {
            ReverseCraftInventory();
        }


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

    private void ReverseCraftInventory()
    {
        int fullSlots = GetUsedInvSlots(curInventory);

        if (fullSlots >= MAX_FULL_CELLS) return;

        GameManager gm = GameManager.GameManagerInstance;
        Dictionary<string, int> newInventory = new Dictionary<string, int>();
        ItemIngredientTable itemRevRecipes;

        foreach (var item in curInventory)
        {
            itemRevRecipes = gm.recipeManager.GetIngredients(item.Key);

            if (itemRevRecipes == null)
            {
                AddToInv(newInventory, item.Key, item.Value);
                continue;
            }

            int remAmt = item.Value;

            for (int i = 0; i < item.Value; i++)
            {
                if (Random.Range(0, 100) > BREAK_DOWN_THRESHOLD) continue;

                Dictionary<string, int> randRevRecipe = itemRevRecipes.table[Random.Range(0, itemRevRecipes.table.Count)];
                // check if full cell limit will be exceeded
                AddRecipeToInv(newInventory, randRevRecipe);
                remAmt--;
            }

            AddToInv(newInventory, item.Key, remAmt);
        }

        curInventory = newInventory;
    }

    private void AddUnhelfulItems()
    {

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

    private void AddRecipeToInv(Dictionary<string, int> inv, Dictionary<string, int> recipe)
    {
        foreach (var item in recipe)
        {
            AddToInv(inv, ReplaceSpecialMetadata(item.Key), item.Value);
        }
    }

    private string ReplaceSpecialMetadata(string resString)
    {
        if (resString.Substring(3, 2) != "99") return resString;

        GameManager gm = GameManager.GameManagerInstance;
        string resId = resString.Substring(0, 3);
        ItemData itemData = gm.itemDataManager.getItemDataById(int.Parse(resId));
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

            itemData = gm.itemDataManager.getItemDataById(int.Parse(item.Key.Substring(0, 3)));

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
}

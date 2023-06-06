using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] TextAsset recipeJson;

    Recipes recipes;
    Dictionary<string, Result>[,] careRecipeConversionGrid;
    Dictionary<string, Result> notCareRecipeConversion;
    Dictionary<string, List<int>> careSpecialMetadata;
    Dictionary<string, ItemIngredientTable> itemIngredients;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        careRecipeConversionGrid = new Dictionary<string, Result>[3, 3];
        for (int row = 0; row < careRecipeConversionGrid.GetLength(0); row++)
        {
            for (int col = 0; col < careRecipeConversionGrid.GetLength(1); col++)
            {
                careRecipeConversionGrid[row, col] = new Dictionary<string, Result>();
            }
        }
        careSpecialMetadata = new Dictionary<string, List<int>>();
        notCareRecipeConversion = new Dictionary<string, Result>();
        itemIngredients = new Dictionary<string, ItemIngredientTable>();

        string jsonString = recipeJson.ToString();
        recipes = JsonUtility.FromJson<Recipes>(jsonString);

        foreach (Recipe r in recipes.careRecipes)
        {
            if (r.specialIndices.Count > 0)
            {
                careSpecialMetadata.Add(r.MakeIdOnlyString(), r.specialIndices);
            }

            string rGridString;
            Dictionary<string, int> rIngredientList;

            var rData = r.GetCombinedRecipeData();
            rGridString = rData.Item1;
            rIngredientList = rData.Item2;

            careRecipeConversionGrid[r.dim.bottomRightX, r.dim.bottomRightY].Add(rGridString, r.result);

            string resultString = r.result.GetResString();

            if (itemIngredients.ContainsKey(resultString))
            {
                itemIngredients[resultString].table.Add(rIngredientList);
            }
            else
            {
                itemIngredients[resultString] = new ItemIngredientTable(rIngredientList);
            }
        }

        foreach (Recipe r in recipes.notCareRecipes)
        {
            notCareRecipeConversion.Add(r.MakeGridString(), r.result);
        }

        IsInitialized = true;
    }

    public Result FindMatch(string craftGridString, BoundingBox dim)
    {
        if (dim == null || craftGridString == "") return null;

        Vector2Int boxSize = new Vector2Int(dim.bottomRightX - dim.topLeftX, dim.bottomRightY - dim.topLeftY);

        if (careRecipeConversionGrid[boxSize.x, boxSize.y].ContainsKey(craftGridString))
        {
            return careRecipeConversionGrid[boxSize.x, boxSize.y][craftGridString];
        }

        string idOnly = ExtractIdString(craftGridString);

        if (careSpecialMetadata.ContainsKey(idOnly))
        {
            string modGridString = ReplaceSpecialIndices(craftGridString, careSpecialMetadata[idOnly]);

            if (careRecipeConversionGrid[boxSize.x, boxSize.y].ContainsKey(modGridString))
            {
                return careRecipeConversionGrid[boxSize.x, boxSize.y][modGridString];
            }
        }

        string sortedGridString = SortGridString(craftGridString);

        if (notCareRecipeConversion.ContainsKey(sortedGridString))
        {
            return notCareRecipeConversion[sortedGridString];
        }
        
        return null;
    }

    private string SortGridString(string gridString)
    {
        List<string> sections = new List<string>();

        if (gridString.Length % 5 != 0)
        {
            Debug.Log("Incorrect gridstring size");
            return null;
        }

        int bounds = gridString.Length / 5;

        for(int i = 0; i < bounds; i++)
        {
            sections.Add(gridString.Substring(i * 5, 5));
        }

        sections.Sort();
        return String.Concat(sections);
    }

    private string ExtractIdString(string gridString)
    {
        string res = "";

        if (gridString.Length % 5 != 0)
        {
            Debug.Log("Incorrect gridstring size");
            return null;
        }

        int bounds = gridString.Length / 5;

        for (int i = 0; i < bounds; i++)
        {
            res += gridString.Substring(i * 5, 3);
        }

        return res;
    }

    private string ReplaceSpecialIndices(string gridString, List<int> specialIndicies)
    {
        string res = "";

        if (gridString.Length % 5 != 0)
        {
            Debug.Log("Incorrect gridstring size");
            return null;
        }

        int bounds = gridString.Length / 5;

        for (int i = 0; i < bounds; i++)
        {
            if (specialIndicies.Contains(i))
            {
                res += gridString.Substring(i * 5, 3) + "99";
            }
            else
            {
                res += gridString.Substring(i * 5, 5);
            }
        }

        return res;
    }
}

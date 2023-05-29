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
        notCareRecipeConversion = new Dictionary<string, Result>();

        string jsonString = recipeJson.ToString();
        recipes = JsonUtility.FromJson<Recipes>(jsonString);

        foreach (Recipe r in recipes.careRecipes)
        {
            careRecipeConversionGrid[r.dim.bottomRightX - r.dim.topLeftX, r.dim.bottomRightY - r.dim.topLeftY].Add(r.MakeGridString(), r.result);
        }

        foreach (Recipe r in recipes.notCareRecipes)
        {
            notCareRecipeConversion.Add(r.MakeGridString(), r.result);
        }

        IsInitialized = true;
    }

    public Result FindMatch(string craftGridString, BoundingBox dim)
    {
        if (dim == null) return null;

        Vector2Int boxSize = new Vector2Int(dim.bottomRightX - dim.topLeftX, dim.bottomRightY - dim.topLeftY);

        if (!careRecipeConversionGrid[boxSize.x, boxSize.y].ContainsKey(craftGridString)) return null;

        Result res = careRecipeConversionGrid[boxSize.x, boxSize.y][craftGridString];

        if (res != null) return res;

        res = notCareRecipeConversion[SortGridString(craftGridString)];

        return res;
    }

    private string SortGridString(string gridString)
    {
        List<string> sections = new List<string>();

        if (gridString.Length % 4 != 0)
        {
            Debug.Log("Incorrect gridstring size");
            return null;
        }

        int bounds = gridString.Length / 4;
        for(int i = 0; i < bounds; i++)
        {
            sections.Add(gridString.Substring(i * 4, 4));
        }

        sections.Sort();
        return String.Concat(sections);
    }
}

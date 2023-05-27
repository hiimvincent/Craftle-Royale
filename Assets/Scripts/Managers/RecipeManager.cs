using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] TextAsset recipeJson;
    Recipes recipes;
    Dictionary<string, Result> recipeConversion;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        recipeConversion = new Dictionary<string, Result>();

        string jsonString = recipeJson.ToString();
        recipes = JsonUtility.FromJson<Recipes>(jsonString);

        foreach (Recipe r in recipes.care_recipes)
        {
            recipeConversion.Add(r.MakeGridString(), r.result);
        }

        IsInitialized = true;
    }

    public Result FindMatch(string craftGridString)
    {
        if (!recipeConversion.ContainsKey(craftGridString)) return null;

        return recipeConversion[craftGridString];
    }
}

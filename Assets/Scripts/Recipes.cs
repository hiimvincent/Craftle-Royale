using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GridEntry
{
    public int id;
    public int metadata;

    public GridEntry(int id, int metadata = 0)
    {
        this.id = id;
        this.metadata = metadata;
    }
}

[System.Serializable]
public class Result
{
    public int id;
    public int metadata;
    public int quantity;

    public Result(int id, int metadata = 0, int quantity = 1)
    {
        this.id = id;
        this.metadata = metadata;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class GridRow
{
    public List<GridEntry> row;

    public GridRow(List<GridEntry> row)
    {
        this.row = row;
    }
}

[System.Serializable]
public class Recipe
{
    public List<GridRow> grid;
    public Result result;

    public Recipe(List<GridRow> grid, Result result)
    {
        this.grid = grid;
        this.result = result;
    }

    public string MakeGridString()
    {
        string res = "";
        for(int row = 0; row < grid.Count; row++)
        {
            for(int col = 0; col < grid[row].row.Count; col++)
            {
                res += grid[row].row[col].id.ToString("D3") + grid[row].row[col].metadata.ToString();
            }
        }

        return res;
    }
}

[System.Serializable]
public class Recipes
{
    public List<Recipe> care_recipes;

    public Recipes(List<Recipe> care_recipes)
    {
        this.care_recipes = care_recipes;
    }
}

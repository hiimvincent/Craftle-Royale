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
public class BoundingBox
{
    public int topLeftX;
    public int topLeftY;
    public int bottomRightX;
    public int bottomRightY;

    public BoundingBox(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
    {
        this.topLeftX = topLeftX;
        this.topLeftY = topLeftY;
        this.bottomRightX = bottomRightX;
        this.bottomRightY = bottomRightY;
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
    public BoundingBox dim;
    public Result result;

    public Recipe(List<GridRow> grid, BoundingBox dim, Result result)
    {
        this.grid = grid;
        this.dim = dim;
        this.result = result;
    }

    public string MakeGridString()
    {
        string res = "";
        for(int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for(int col = dim.topLeftY; col <= dim.bottomRightY; col++)
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
    public List<Recipe> careRecipes;
    public List<Recipe> notCareRecipes;

    public Recipes(List<Recipe> careRecipes, List<Recipe> notCareRecipes)
    {
        this.careRecipes = careRecipes;
        this.notCareRecipes = notCareRecipes;
    }
}

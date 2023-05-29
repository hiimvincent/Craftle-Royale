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
    public int bottomRightX;
    public int bottomRightY;
    public int topLeftX;
    public int topLeftY;

    public BoundingBox(int bottomRightX, int bottomRightY, int topLeftX = 0, int topLeftY = 0)
    {
        this.bottomRightX = bottomRightX;
        this.bottomRightY = bottomRightY;
        this.topLeftX = topLeftX;
        this.topLeftY = topLeftY;
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
    public List<int> specialIndices;

    public Recipe(List<GridRow> grid, BoundingBox dim, Result result, List<int> specialIndices = null)
    {
        this.grid = grid;
        this.dim = dim;
        this.result = result;
        this.specialIndices = specialIndices;

    }

    public string MakeGridString()
    {
        string res = "";
        for(int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for(int col = dim.topLeftY; col <= dim.bottomRightY; col++)
            {
                res += grid[row].row[col].id.ToString("D3") + grid[row].row[col].metadata.ToString("D2");
            }
        }

        return res;
    }

    public string MakeIdOnlyString()
    {
        string res = "";
        for (int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for (int col = dim.topLeftY; col <= dim.bottomRightY; col++)
            {
                res += grid[row].row[col].id.ToString("D3");
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

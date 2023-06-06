using System.Collections.Generic;

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

    public string GetResString()
    {
        return id.ToString("D3") + metadata.ToString("D2");
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
public class ItemIngredientTable
{
    public List<Dictionary<string, int>> table;

    public ItemIngredientTable()
    {
        table = new List<Dictionary<string, int>>();
    }

    public ItemIngredientTable(Dictionary<string, int> firstEntry) : this()
    {
        table.Add(firstEntry);
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
        for (int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for (int col = dim.topLeftY; col <= dim.bottomRightY; col++)
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

    public Dictionary<string, int> GetIngredientDict() {
        Dictionary<string, int> res = new Dictionary<string, int>();

        for (int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for (int col = dim.topLeftY; col <= dim.bottomRightY; col++)
            {
                if (grid[row].row[col].id == 0) continue;

                string gridCellString = grid[row].row[col].id.ToString("D3") + grid[row].row[col].metadata.ToString("D2");

                if (res.ContainsKey(gridCellString))
                {
                    res[gridCellString] += 1;
                }
                else
                {
                    res[gridCellString] = 0;
                }
            }
        }

        return res;
    }

    public (string, Dictionary<string, int>) GetCombinedRecipeData()
    {
        string gridString = "";
        Dictionary<string, int> ingredients = new Dictionary<string, int>();

        for (int row = dim.topLeftX; row <= dim.bottomRightX; row++)
        {
            for (int col = dim.topLeftY; col <= dim.bottomRightY; col++)
            {
                gridString += grid[row].row[col].id.ToString("D3") + grid[row].row[col].metadata.ToString("D2");

                if (grid[row].row[col].id == 0) continue;

                string cellString = grid[row].row[col].id.ToString("D3") + grid[row].row[col].metadata.ToString("D2");

                if (ingredients.ContainsKey(cellString))
                {
                    ingredients[cellString] += 1;
                }
                else
                {
                    ingredients[cellString] = 0;
                }
            }
        }

        return (gridString, ingredients);
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

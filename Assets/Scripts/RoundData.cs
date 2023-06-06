using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemPoolItem
{
    public int id;
    public int metadata;

    public ItemPoolItem(int id, int metadata = 0)
    {
        this.id = id;
        this.metadata = metadata;
    }
}

[System.Serializable]
public class RoundData
{
    public List<ItemPoolItem> itemPool;

    public RoundData(List<ItemPoolItem> itemPool)
    {
        this.itemPool = itemPool;
    }
}

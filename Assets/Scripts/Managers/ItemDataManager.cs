using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    [SerializeField] private ItemData[] dataList;
    Dictionary<int, ItemData> dataIdConversion;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        dataIdConversion = new Dictionary<int, ItemData>();
        foreach(ItemData cur in dataList)
        {
            dataIdConversion.Add(cur.id, cur);
        }

        IsInitialized = true;
    }

    public ItemData GetItemDataById(int id)
    {
        return dataIdConversion[id];
    }

    public List<ItemData> GetRandomItemDatas(int size)
    {
        if (size <= 0) return null;

        List<ItemData> res = new List<ItemData>();

        for (int i = 0; i < size; i++)
        {
            res.Add(dataList[Random.Range(0, dataList.Length)]);
        }

        return res;
    }
}

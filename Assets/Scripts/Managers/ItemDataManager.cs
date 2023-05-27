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

    public ItemData getItemDataById(int id)
    {
        return dataIdConversion[id];
    }
}

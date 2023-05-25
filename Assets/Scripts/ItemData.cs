using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string[] names;
    public Sprite[] images;
    public ItemType type = ItemType.Stackable;
    public int stackableLimit = 64;
    

    public enum ItemType
    {
        Stackable, NonStackable
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object")]
public class ItemData : ScriptableObject
{
    public Sprite image;
    public int id;
    public ItemType type = ItemType.Stackable;
    public int stackableLimit = 64;
    

    public enum ItemType
    {
        Stackable, NonStackable
    }
}

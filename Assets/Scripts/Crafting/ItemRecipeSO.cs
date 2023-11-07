using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Recipe", menuName ="Scriptable Objects/Item Recipe")]

public class ItemRecipeSO : ScriptableObject
{
    public string RecipeName;
    public RecipeType TypeOfRecipe;
    public ItemTypeAndCount[] input;
    public ItemTypeAndCount[] output;
}

public enum RecipeType { Smelting, Assembling};

[System.Serializable]

public class ItemTypeAndCount
{
    public InventoryItemData item;
    public int count;


    public ItemTypeAndCount(InventoryItemData i, int c)
    {
        item = i;
        count = c;
    }
}
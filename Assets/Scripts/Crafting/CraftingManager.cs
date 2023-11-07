using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    //https://www.youtube.com/watch?v=wShDVdOIRG8

    private List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();
    private BuildingInventory inventory;
    [SerializeField] private ItemRecipeSO testRecipe;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            CanCraftRecipe(testRecipe);
            Debug.Log(CanCraftRecipe(testRecipe));
        }
    }

    private bool CanCraftRecipe(ItemRecipeSO recipeSO)
    {
        inventory = gameObject.GetComponent<BuildingInventory>();
        items = inventory.GetAllItems();
        int foundItems = 0;
        foreach (ItemTypeAndCount neededItemsAndCount in recipeSO.input) 
        {
        foreach(ItemTypeAndCount foundItemAndCount in items)
            {
                if(foundItemAndCount.item == neededItemsAndCount.item && foundItemAndCount.count >= neededItemsAndCount.count) 
                { 
                    foundItems++;
                    break;
                }
            }
        }

        return foundItems == recipeSO.input.Length;
    }
}

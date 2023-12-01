using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    //https://www.youtube.com/watch?v=wShDVdOIRG8

    private List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();
    private BuildingInventory inventory;
    [SerializeField] private ItemRecipeSO RecipeToCraft;
    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.H))
       {
           if(CanCraftRecipe(RecipeToCraft))
           {
               CraftItems(new List<ItemTypeAndCount>(RecipeToCraft.output), new List<ItemTypeAndCount>(RecipeToCraft.input));
           }
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


    public void CraftItems(List<ItemTypeAndCount> itemsToCraft, List<ItemTypeAndCount> itemsToDestroy)
    {
        foreach(ItemTypeAndCount itemToDestroy in itemsToDestroy)
        {
            for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
            {
                if (inventory.InventorySystem.InventorySlots[i].ItemData != null && inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == itemToDestroy.item.resourceType)
                {
                    inventory.InventorySystem.InventorySlots[i].ClearSlot();
                }
            }
        }

        foreach (ItemTypeAndCount itemToCraft in itemsToCraft)
        {
            inventory.InventorySystem.AddToInventory(itemToCraft.item, 1);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay inventoryPanel;

    private void Awake()
    {
        inventoryPanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
    }
    void Update()
    {
        if (inventoryPanel.gameObject.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape)))
            {
            inventoryPanel.gameObject.SetActive(false);
            }
    }

    public void DisplayInventory(InventorySystem invToDislpay) 
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventory(invToDislpay);
    }

}

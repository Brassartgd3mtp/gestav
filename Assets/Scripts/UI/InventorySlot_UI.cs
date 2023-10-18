using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventorySlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;

    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;

    private void Awake()
    {
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = string.Empty;

        button.GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);
    }

    public void Init(InventorySlot _slot)
    {
        assignedInventorySlot = _slot;
        UpdateUISlot(_slot);
    }

    public void UpdateUISlot(InventorySlot _slot)
    {
        if(_slot.ItemData != null)
        {
            itemSprite.sprite = _slot.ItemData.icon;
            itemSprite.color = Color.clear;
        }
        else
        {
            ClearSlot();
        }
    }


    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = string.Empty;
    }
    public void OnUISlotClick()
    {
        //access display class method
    }
}

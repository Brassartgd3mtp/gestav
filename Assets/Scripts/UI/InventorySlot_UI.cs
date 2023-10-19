using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;

    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }
    private void Awake()
    {

        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
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
            itemSprite.color = Color.white;

            if (_slot.StackSize > 1)
            {
                itemCount.text = _slot.StackSize.ToString();
            }
            else
            {
                itemCount.text = string.Empty;
            }

        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if(assignedInventorySlot != null) 
        {
            UpdateUISlot(assignedInventorySlot);
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
        ParentDisplay?.SlotClicked(this);
    }
}

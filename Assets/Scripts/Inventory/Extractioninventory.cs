using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class Extractioninventory : MonoBehaviour
{
    private InventoryHolder workerInventory;
    private InventoryHolder thisInventory;
    private CharacterManager characterManager;
    private int resourcesExtracted = 0;
    private int amountToExtract;
    private bool amountExctracted;
    [SerializeField] private GameObject VictoryUI;
    [SerializeField] private TextMeshProUGUI currentResourcesText;
    [SerializeField] private TextMeshProUGUI totalResourcesText;

    private void Awake()
    {
        thisInventory = GetComponent<InventoryHolder>();
        amountExctracted = false;
        amountToExtract = 5;
        VictoryUI.SetActive(false);
        totalResourcesText.text = amountToExtract.ToString();
        currentResourcesText.text = resourcesExtracted.ToString();
    }
    private async void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterManager>()  != null)
        {
            characterManager = other.GetComponent<CharacterManager>();
            Debug.Log("Character Manager trouvé");
            workerInventory = other.GetComponentInChildren<InventoryHolder>();

            for (int i = 0; i < workerInventory.InventorySystem.InventorySlots.Count; i++)
            {
                if (workerInventory.InventorySystem.InventorySlots[i].ItemData != null)
                {
                    await Task.Delay(characterManager.DepositDuration);

                    //Do the inventory transfer
                    thisInventory.InventorySystem.InventorySlots.Add(new InventorySlot());
                    thisInventory.InventorySystem.AddToInventory(workerInventory.InventorySystem.InventorySlots[i].ItemData, 1);
                    workerInventory.InventorySystem.InventorySlots[i].ClearSlot();

                    resourcesExtracted++;
                    Debug.Log(resourcesExtracted);

                    //Update the bag and the UI
                    characterManager.ChangeBagSize(characterManager.CalculateBagSize());
                    characterManager.DisplayThisIventory();
                    currentResourcesText.text = resourcesExtracted.ToString(); 
                }
                if(resourcesExtracted >= amountToExtract)
                {
                    amountExctracted=true;
                    VictoryUI.SetActive(true);
                }
            }
            //hide the worker's bag if their inv is empty
            characterManager.HideBag();

        }
    }

}

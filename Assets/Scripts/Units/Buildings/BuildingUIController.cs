using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BuildingUIController : MonoBehaviour
{
    public GameObject ActionsPopUpPanel;
    private BuildingManager unitAssociated;
    public GameObject BuildPopUpPanel;

    private void Awake()
    {
       unitAssociated = GetComponent<BuildingManager>();
       HideActionsUIPopUp();
       HideBuildPopUp();
    }

    private void Update()
    {
        if (Global.SELECTED_BUILDINGS.Contains(unitAssociated) && Global.SELECTED_BUILDINGS.Count == 1 && unitAssociated.hasBeenBuilt)
        {
            DisplayActionsUIPopUp();
            HideBuildPopUp();
        }
        else if (Global.SELECTED_BUILDINGS.Contains(unitAssociated) && Global.SELECTED_BUILDINGS.Count == 1 && !unitAssociated.hasBeenBuilt)
        {
            HideActionsUIPopUp();
            DisplayBuildPopUp();
        } 
        else
        {
            HideBuildPopUp();
            HideActionsUIPopUp();
        }

    }

    public void DisplayActionsUIPopUp()
    {
        if(ActionsPopUpPanel != null) ActionsPopUpPanel.gameObject.SetActive(true);
    }

    public void HideActionsUIPopUp()
    {
        if (ActionsPopUpPanel != null) ActionsPopUpPanel.gameObject.SetActive(false);
    }

    public void DisplayBuildPopUp()
    {
        if(BuildPopUpPanel != null) BuildPopUpPanel.gameObject.SetActive(true);
    }

    public void HideBuildPopUp()
    {
        if (BuildPopUpPanel != null) BuildPopUpPanel.gameObject.SetActive(false);
    } 
}

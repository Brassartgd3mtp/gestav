using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BuildingUIController : MonoBehaviour
{
    public GameObject ActionsPopUpPanel;
    private UnitManager unitAssociated;

    private void Awake()
    {
        unitAssociated = GetComponent<UnitManager>();
        HideBuildingUIPopUp();
    }

    private void Update()
    {
        if (Global.SELECTED_UNITS.Contains(unitAssociated) /*&& Global.SELECTED_UNITS.Count ==1*/)
        {
            DisplayBuildingUIPopUp();
        }
        else HideBuildingUIPopUp();
    }

    public void DisplayBuildingUIPopUp()
    {
        ActionsPopUpPanel.gameObject.SetActive(true);
    }

    public void HideBuildingUIPopUp()
    {
        ActionsPopUpPanel.gameObject.SetActive(false);
    }
}

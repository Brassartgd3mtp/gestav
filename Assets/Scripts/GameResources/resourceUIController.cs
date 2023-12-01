using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resourceUIController : MonoBehaviour
{
    public GameObject ActionsPopUpPanel;

    private void Awake()
    {
        HideActionsUIPopUp();
    }
    private void OnMouseDown()
    {
        if (ActionsPopUpPanel.activeSelf) { HideActionsUIPopUp(); }
        else { DisplayActionsUIPopUp(); }
    }
    public void DisplayActionsUIPopUp()
    {
        if (ActionsPopUpPanel != null) ActionsPopUpPanel.gameObject.SetActive(true);
    }

    public void HideActionsUIPopUp()
    {
        if (ActionsPopUpPanel != null) ActionsPopUpPanel.gameObject.SetActive(false);
    }
}
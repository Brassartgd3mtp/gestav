using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private GameObject workerUIGameObject;

    [SerializeField] private Button removeAssignationButton;

    private AssignWorker _assignedBuildingWorkerInv;
    private BuildingActionSelection buildingActionSelection;

    private void Awake()
    {
        HideWorkerUI();
    }

    public void ShowWorkerUI()
    {
        workerUIGameObject.SetActive(true);
    }

    public void HideWorkerUI()
    {
        workerUIGameObject.SetActive(false);
    }

    public void RemoveAssignation()
    {
        foreach (UnitManager Unit in Global.SELECTED_CHARACTERS)
        {
            CharacterManager _characterManagerRef = Unit.GetComponent<CharacterManager>();
            if(_characterManagerRef != null)
            {
                BuildingManager _assignedBuildingRef = _characterManagerRef.buildingAssigned;
                if (_characterManagerRef != null && _assignedBuildingRef != null)
                {

                    _assignedBuildingWorkerInv = _assignedBuildingRef.GetComponent<AssignWorker>();

                    _assignedBuildingWorkerInv.RemoveWorkers();

                }
            }
         


        }
    }
}

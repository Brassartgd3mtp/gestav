using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private GameObject workerUIGameObject;

    [SerializeField] private Button removeAssignationButton;

    private AssignWorkerInventory _assignedBuildingWorkerInv;

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
            WorkerManager _workerManagerRef = Unit.GetComponent<WorkerManager>();
            if(_workerManagerRef != null)
            {
                ResourceSpotUI _assignedResourceRef = _workerManagerRef.resourceAssigned;
                BuildingManager _assignedBuildingRef = _workerManagerRef.buildingAssigned;
                RemoveOneWorker(_workerManagerRef, _assignedBuildingRef, _assignedResourceRef);
            }
        }
    }

    public void RemoveOneWorker(CharacterManager _characterManagerRef, BuildingManager _assignedBuildingRef, ResourceSpotUI _assignedResourceRef)
    {
        if (_characterManagerRef != null)
        {
            if (_assignedBuildingRef != null)
            _assignedBuildingWorkerInv = _assignedBuildingRef.GetComponent<AssignWorkerInventory>();

            if(_assignedResourceRef != null)
            _assignedBuildingWorkerInv = _assignedResourceRef.GetComponent<AssignWorkerInventory>();

            _assignedBuildingWorkerInv.RemoveWorkers();

        }
    }
}

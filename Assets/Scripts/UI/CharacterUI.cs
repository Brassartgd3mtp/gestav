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
            CharacterManager _characterManagerRef = Unit.GetComponent<CharacterManager>();
            if(_characterManagerRef != null)
            {
                ResourceSpotUI _assignedResourceRef = _characterManagerRef.resourceAssigned;
                BuildingManager _assignedBuildingRef = _characterManagerRef.buildingAssigned;
                RemoveOneWorker(_characterManagerRef, _assignedBuildingRef, _assignedResourceRef);
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

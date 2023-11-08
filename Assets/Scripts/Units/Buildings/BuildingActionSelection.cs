using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BuildingActionSelection : MonoBehaviour
{
    private CharacterManager characterManager;
    private AssignWorker assignWorker;

    private void Awake()
    {
        assignWorker = gameObject.GetComponentInParent<AssignWorker>();
    }
    public void UseBuilding()
    {
        assignWorker.AddWorkersToList();
    }
    
}

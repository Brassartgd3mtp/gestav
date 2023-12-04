using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class workerButton : MonoBehaviour
{

    private CharacterManager characterManager;
    private void OnMouseEnter()
    {
        Debug.Log("Mouse Enter");
        foreach (GameObject go in GetAllWorkersAssigned())
        {
            Blink blink = gameObject.GetComponentInChildren<Blink>();
            blink.StartBlinking();
        }
    }

    private void OnMouseExit()
    {
        foreach (GameObject go in GetAllWorkersAssigned())
        {
            WorkerManager workerManagerRef = go.GetComponent<WorkerManager>();
            BuildingManager buildingManager = gameObject.GetComponentInParent<BuildingManager>();
            if(workerManagerRef.buildingAssigned == buildingManager) 
            {
                Blink blink = go.GetComponentInChildren<Blink>();
                blink.StartBlinking();
            }
        }
    }

    public GameObject[] GetAllWorkersAssigned()
    {
        float detectionRadius = 10000f;
        Collider[] Colliders = Physics.OverlapSphere(transform.position, detectionRadius, Global.WORKER_LAYER_MASK);
        GameObject[] go = new GameObject[Colliders.Length];
        
        return go;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceActionSelection : MonoBehaviour
{
    private AssignWorkerInventory assignWorker;

    private void Awake()
    {
        assignWorker = gameObject.GetComponentInParent<AssignWorkerInventory>();
    }

    public void Collect()
    {
        assignWorker.AddWorkersToList();
    }
}

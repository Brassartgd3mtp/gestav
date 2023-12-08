using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    private EnemySpawner spawnerScript;

    private void Awake()
    {
        spawnerScript = GetComponentInParent<EnemySpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision");
        other.gameObject.TryGetComponent(out UnitManager um);
        if (um != null)
        {
            spawnerScript.unitsInArea++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent(out UnitManager um);
        if (um != null)
        {
            spawnerScript.unitsInArea--;
        }
    }
}

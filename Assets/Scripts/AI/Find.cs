using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Find : MonoBehaviour
{

    private float detectionRadius = 10000f;

    public Transform GetClosestBuilding(Transform[] _transform)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in _transform)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;

    }

    public Transform[] GetTransformArray(LayerMask _buildingLayerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, _buildingLayerMask);
        Transform[] buildingTransform = new Transform[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            buildingTransform[i] = colliders[i].transform;
        }
        return buildingTransform;
    }

    public Transform[] GetCorsspondingBuilding(Transform[] _transform, string _buildingName)
    {
        for (int i = 0; i < _transform.Length; i++)
        {
            _transform[i].Find(_buildingName);
        }
        return _transform;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject currentTarget;
    [SerializeField] private Collider[] colliders;

    [Space]
    [SerializeField] private int detectionRange = 12;
    [SerializeField] private int damageRange = 2;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private LayerMask detectable;

    [Space]
    private NavMeshAgent agent;
    private float compareDistance = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (currentTarget == null)
        {
            if (Detect())
            {
                StartCoroutine(ApplyDamage());
            }
            else
                Debug.Log("Target not found !");
        }       
        else
        {
            compareDistance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (compareDistance > detectionRange)
            {
                currentTarget = null;
            }
        }
    }

    private bool Detect()
    {
        Debug.Log("Start detection");
        Collider[] hit = Physics.OverlapSphere(transform.position, detectionRange, detectable);
        colliders = hit;

        float distanceMin = float.MaxValue;
        GameObject nearestObject = null;

        foreach (Collider hitCollider in hit)
        {
            if (hitCollider.TryGetComponent(out BuildingStockageUI _bsui))
            {
                float nearestCollider = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (nearestCollider < distanceMin)
                {
                    distanceMin = nearestCollider;
                    nearestObject = _bsui.gameObject;
                }
            }
            else if (hitCollider.TryGetComponent(out CharacterManager _cm))
            {
                float nearestCollider = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (nearestCollider < distanceMin)
                {
                    distanceMin = nearestCollider;
                    nearestObject = _cm.gameObject;
                }
            }
            else
            {
                Debug.LogError($"{hitCollider} don't have any of the required component !");
                return false;
            }
        }

        if (hit.Count() > 0 && nearestObject != null)
        {
            currentTarget = nearestObject;
            Debug.Log($"New target is {currentTarget} !");
            if (currentTarget != null)
                StartCoroutine(FollowTarget());
            return true;
        }
        else
            return false;
    }

    private IEnumerator ApplyDamage()
    {
        while(true)
        {
            while (compareDistance <= damageRange)
            {
                yield return new WaitForSeconds(attackCooldown);

                if (currentTarget != null)
                {
                    if (currentTarget.TryGetComponent(out UnitManager _um))
                        _um.HealthPoints -= attackDamage;
                }
                else
                    yield break;

                yield return null;
            }
            yield return null;
        }
    }

    private IEnumerator FollowTarget()
    {
        while (true)
        {
            if (currentTarget != null)
            {
                if (compareDistance <= detectionRange)
                {
                    agent.destination = currentTarget.transform.position;
                }
            }
            else
                yield break;

            yield return new WaitForSeconds(.1f);
        }
    }

    //private Vector3 OffsetPosition(GameObject _gameObject)
    //{
    //    return _gameObject.transform.position + _gameObject.GetComponent<BoxCollider>().size;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}

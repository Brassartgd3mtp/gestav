using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject currentTarget;

    [Space]
    [SerializeField] private int detectionRange = 12;
    [SerializeField] private int damageRange = 2;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private LayerMask detectable;

    [Space]
    private NavMeshAgent agent;
    float compareDistance = 0;

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
        }
        else
            compareDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    private bool Detect()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, detectionRange, detectable);

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

                agent.SetDestination(nearestObject.transform.position);
                currentTarget = nearestObject;
                Debug.Log("Building Found !");
                return true;
            }
            else if (hitCollider.TryGetComponent(out CharacterManager _cm))
            {
                float nearestCollider = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (nearestCollider < distanceMin)
                {
                    distanceMin = nearestCollider;
                    nearestObject = _cm.gameObject;
                }

                agent.SetDestination(nearestObject.transform.position);
                currentTarget = nearestObject;
                Debug.Log("Worker Found !");
                return true;
            }
            else
                Debug.LogWarning("Object don't have any of the needed component.");
        }
        return false;
    }

    private IEnumerator ApplyDamage()
    {
        while(true)
        {
            while (compareDistance <= damageRange)
            {
                if (currentTarget != null)
                {
                    yield return new WaitForSeconds(attackCooldown);

                    if (currentTarget.TryGetComponent(out BuildingStockageUI _bsui))
                        _bsui.HealthPoints -= attackDamage;
                    else if (currentTarget.TryGetComponent(out CharacterManager _cm))
                        _cm.HealthPoints -= attackDamage;
                }
                else
                    yield break;

                yield return null;
            }
            yield return null;
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

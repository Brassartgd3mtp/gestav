using System.Buffers;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public interface IState
{
    public void Enter();
    public void Execute();
    public void Exit();
}

public class EnemySM
{
    IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null) currentState.Execute();
    }
}

public class Wandering : IState
{
    EnemyAI owner;
    public Wandering(EnemyAI owner)
    {
        this.owner = owner;
    }

    public float wanderingDelay;
    private int wanderingIndex;

    private Vector3 basePos;
    private Vector3[] wanderingSpotPos =
    {
        new Vector3(10, 0, 0),
        new Vector3(0, 0, 10),
        new Vector3(-10, 0, 0),
        new Vector3(0, 0, -10)
    };

    public void Enter()
    {
        owner.SetState(0);

        wanderingDelay = Random.Range(0, 8);
        wanderingIndex = Random.Range(0, 4);
        basePos = owner.transform.position;
    }

    public void Execute()
    {
        Debug.Log("execute Wandering state");
        if (wanderingDelay <= 0)
        {
            wanderingDelay = 9;
            owner.Agent.SetDestination(basePos + wanderingSpotPos[wanderingIndex]);
        }
        else if (wanderingDelay <= 8 && wanderingDelay > 0)
            wanderingDelay -= Time.deltaTime;

        if (owner.Agent.pathStatus == NavMeshPathStatus.PathComplete)
            owner.EnemyStateMachine.ChangeState(owner.Detect);
    }

    public void Exit()
    {
        return;
    }
}

public class Detect : IState
{
    EnemyAI owner;
    Collider[] hit;
    float distanceMin;
    GameObject nearestObject;

    public Detect(EnemyAI owner)
    {
        this.owner = owner;
    }
    public void Enter()
    {
        owner.SetState(1);

        hit = Physics.OverlapSphere(owner.transform.position, owner.DetectionRange, owner.Detectable);

        if (hit.Count() > 0)
        {
            distanceMin = float.MaxValue;
            nearestObject = null;

            Execute();
        }
        else
            owner.EnemyStateMachine.ChangeState(owner.Wandering);
    }
    
    public void Execute()
    {
        Debug.Log("execute Detect state");
        foreach (Collider hitCollider in hit)
        {
            if (hitCollider.TryGetComponent(out BuildingStockageUI _bsui))
            {
                float nearestCollider = Vector3.Distance(owner.transform.position, hitCollider.transform.position);

                if (nearestCollider < distanceMin)
                {
                    distanceMin = nearestCollider;
                    nearestObject = _bsui.gameObject;
                }
            }
            else if (hitCollider.TryGetComponent(out CharacterManager _cm))
            {
                float nearestCollider = Vector3.Distance(owner.transform.position, hitCollider.transform.position);

                if (nearestCollider < distanceMin)
                {
                    distanceMin = nearestCollider;
                    nearestObject = _cm.gameObject;
                }
            }
            else
            {
                Debug.LogError($"{hitCollider} don't have any of the required component !");
            }
        }

        if (hit.Count() > 0 && nearestObject != null)
        {
            owner.CurrentTarget = nearestObject;
            //Debug.Log($"New target is {owner.CurrentTarget} !");

            if (owner.CurrentTarget != null)
            {
                if (owner.CurrentTarget.TryGetComponent(out BuildingStockageUI bsui))
                    owner.EnemyStateMachine.ChangeState(owner.StandAttack);
                else if (owner.CurrentTarget.TryGetComponent(out CharacterManager cm))
                    owner.EnemyStateMachine.ChangeState(owner.FollowAttack);
            }
        }
    }
    
    public void Exit()
    {
        return;
    }
}

public class Follow : IState
{
    EnemyAI owner;

    public Follow(EnemyAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        owner.SetState(2);

        Debug.Log($"{owner.gameObject.name} is entering Follow");
        if (owner.CurrentTarget == null)
            owner.EnemyStateMachine.ChangeState(owner.Detect);
    }

    public void Execute()
    {
        if (owner.CompareDistance <= owner.DetectionRange)
        {
            owner.Agent.destination = owner.CurrentTarget.transform.position;

            if (owner.CompareDistance <= owner.DamageRange)
            {
                if (owner.CurrentTarget.TryGetComponent(out BuildingStockageUI bsui))
                    owner.EnemyStateMachine.ChangeState(owner.StandAttack);
                else if (owner.CurrentTarget.TryGetComponent(out CharacterManager cm))
                    owner.EnemyStateMachine.ChangeState(owner.FollowAttack);
            }
        }
    }

    public void Exit()
    {
        return;
    }
}

public class FollowAttack : IState
{
    EnemyAI owner;

    public FollowAttack(EnemyAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        owner.SetState(3);

        Debug.Log($"{owner.gameObject.name} is entering FollowAttack");
        owner.AttackCooldown = owner.MaxAttackCooldown;
    }

    public void Execute()
    {
        if (owner.CompareDistance <= owner.DetectionRange)
            owner.Agent.destination = owner.CurrentTarget.transform.position;

        if (owner.CompareDistance <= owner.DamageRange)
        {
            if (owner.AttackCooldown <= 0)
            {
                if (owner.CurrentTarget != null)
                {
                    if (owner.CurrentTarget.TryGetComponent(out BuildingStockageUI _bsui))
                        _bsui.HealthPoints -= owner.AttackDamage;
                    else if (owner.CurrentTarget.TryGetComponent(out CharacterManager _cm))
                        _cm.HealthPoints -= owner.AttackDamage;

                    owner.AttackCooldown = owner.MaxAttackCooldown;
                }
                else
                    owner.EnemyStateMachine.ChangeState(owner.Detect);
            }
            else
                owner.AttackCooldown -= Time.deltaTime;
        }
    }

    public void Exit()
    {
        return;
    }
}

public class StandAttack : IState
{
    EnemyAI owner;

    public StandAttack(EnemyAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        owner.SetState(4);

        Debug.Log($"{owner.gameObject.name} is entering StandAttack");
        owner.AttackCooldown = owner.MaxAttackCooldown;

        owner.Agent.destination = owner.CurrentTarget.transform.position;
    }

    public void Execute()
    {
        if (owner.AttackCooldown <= 0)
        {
            if (owner.CurrentTarget != null)
            {
                if (owner.CurrentTarget.TryGetComponent(out BuildingStockageUI _bsui))
                    _bsui.HealthPoints -= owner.AttackDamage;
                else if (owner.CurrentTarget.TryGetComponent(out CharacterManager _cm))
                    _cm.HealthPoints -= owner.AttackDamage;

                owner.AttackCooldown = owner.MaxAttackCooldown;
            }
            else
                owner.EnemyStateMachine.ChangeState(owner.Detect);
        }
        else
            owner.AttackCooldown -= Time.deltaTime;
    }

    public void Exit()
    {
        return;
    }
}
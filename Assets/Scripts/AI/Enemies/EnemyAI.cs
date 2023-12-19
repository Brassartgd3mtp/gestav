using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject CurrentTarget;

    [Space]
    public int DetectionRange = 12;
    public int DamageRange = 2;
    public int AttackDamage = 2;
    public float MaxAttackCooldown = 3f;
    public LayerMask Detectable;
    [HideInInspector] public float AttackCooldown = 3f;

    [Space]
    [Tooltip("Don't modify this variable in Runtime !")]
    public float CompareDistance = 0;
    [Tooltip("Don't modify this variable in Runtime !")]
    public CurrentState State = new CurrentState();
    [Tooltip("Don't modify this variable in Runtime !")]
    public Vector3 spawnPos;
    [Tooltip("Don't modify this variable in Runtime !")]
    public bool isWandering = false;
    [HideInInspector] public NavMeshAgent Agent;

    public EnemySM EnemyStateMachine = new EnemySM();
    public Wandering Wandering;
    public Detect Detect;
    public DefineTargetType DefineTargetType;
    public FollowAttack FollowAttack;
    public StandAttack StandAttack;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        spawnPos = transform.position;

        Wandering = new Wandering(this);
        Detect = new Detect(this);
        DefineTargetType = new DefineTargetType(this);
        FollowAttack = new FollowAttack(this,GetComponent<EnemyManager>());
        StandAttack = new StandAttack(this, GetComponent<EnemyManager>());

        EnemyStateMachine.ChangeState(Detect);
    }

    private void FixedUpdate()
    {
        EnemyStateMachine.Update();

        if (CurrentTarget != null)
        {
            if (State != CurrentState.StandAttack)
            {
                CompareDistance = Agent.remainingDistance;

                if (Agent.hasPath && CompareDistance > DetectionRange)
                {
                    Debug.LogWarning("Target is out of range !");
                    CurrentTarget = null;
                }
            }
        }
        else
            EnemyStateMachine.ChangeState(Detect);
    }

    public void SetState(int _cs)
    {
        State = (CurrentState)_cs;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DamageRange);
    }
}

public enum CurrentState
{
    Wandering,
    Detect,
    DefineTargetType,
    FollowAttack,
    StandAttack
}

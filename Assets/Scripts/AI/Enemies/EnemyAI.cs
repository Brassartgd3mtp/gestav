using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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
    public CurrentState State = new CurrentState();
    public float wanderingDelay;
    [HideInInspector] public NavMeshAgent Agent;

    public EnemySM EnemyStateMachine = new EnemySM();
    public Wandering Wandering;
    public Detect Detect;
    public Follow Follow;
    public FollowAttack FollowAttack;
    public StandAttack StandAttack;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Wandering = new Wandering(this);
        Detect = new Detect(this);
        Follow = new Follow(this);
        FollowAttack = new FollowAttack(this);
        StandAttack = new StandAttack(this);

        EnemyStateMachine.ChangeState(Detect);
    }

    private void FixedUpdate()
    {
        wanderingDelay = Wandering.wanderingDelay;
        if (CurrentTarget != null)
        {
            CompareDistance = Vector3.Distance(transform.position, CurrentTarget.transform.position);

            if (CompareDistance > DetectionRange)
            {
                Debug.LogWarning("Target is out of range !");
                CurrentTarget = null;
            }

            EnemyStateMachine.Update();
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
    Follow,
    FollowAttack,
    StandAttack
}

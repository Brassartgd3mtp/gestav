using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static HeroAIC;

public class HeroAIAttack : HeroBehaviour
{
    private float timer;
    private bool isExecuting = false;
    protected override void Awake()
    {
        base.Awake();
        timer = 0f;
    }

    private void Update()
    {
        if (timer > 0) 
        {
        timer -= Time.deltaTime;
        }
    }

    public override async void ApplyBehaviour()
    {
        HeroManagerRef.canBeMovedByPlayer = true;
        

        if(HeroManagerRef.CurrentTarget != null)
        {
            if (!HeroManagerRef.agent.hasPath)
            {
                Debug.Log("Creating path");
                HeroManagerRef.MoveTo(HeroManagerRef.CurrentTarget.transform.position, HeroManagerRef.CurrentTarget.GetComponent<BoxCollider>().size.z + HeroManagerRef.AttackRange / 2);
            }
            else if (Vector3.Distance(transform.position, HeroManagerRef.CurrentTarget.transform.position) < HeroManagerRef.AttackRange + HeroManagerRef.CurrentTarget.GetComponent<BoxCollider>().size.z)
            {
                Debug.Log("In range");
                if (timer <= 0f)
                {
                    Debug.Log("can attack");
                    Attack();
                }
            }
            else if(!isExecuting)
            {
                Debug.Log("Checking for new path");
                isExecuting = true;
                Vector3 targetPos;
                targetPos = HeroManagerRef.CurrentTarget.transform.position;
                await Task.Delay(500);
                Vector3 nextTargetPos = HeroManagerRef.CurrentTarget.transform.position;

                if (Vector3.Distance(nextTargetPos, transform.position) > Vector3.Distance(targetPos, transform.position))
                {
                    HeroManagerRef.agent.isStopped = true;
                    HeroManagerRef.agent.ResetPath();
                }
                isExecuting = false;
            }

        }

    }


    public override BehaviourName CheckTransition()
    {
        if (HeroManagerRef.CurrentTarget == null)
        {
            if(Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()))
            return BehaviourName.Controlled;
            else return BehaviourName.Wait;
        }
        else return BehaviourName.None;
    }

    public void Attack()
    {
        HeroManagerRef.Animator.SetTrigger("Attack");
        timer = 1 / HeroManagerRef.AttackSpeed;
        HeroManagerRef.gameObject.transform.LookAt(HeroManagerRef.CurrentTarget.transform.position);
        StartCoroutine(ApplyDamage());
    }
    private IEnumerator ApplyDamage() 
    {
        yield return new WaitForSeconds(0.3f);
        HeroManagerRef.InflictDamage(HeroManagerRef.CurrentTarget, HeroManagerRef.Attack);
    }
}

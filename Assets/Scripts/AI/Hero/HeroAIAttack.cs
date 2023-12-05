using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static HeroAIC;

public class HeroAIAttack : HeroBehaviour
{
    private float timer;

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

    public override void ApplyBehaviour()
    {
        HeroManagerRef.canBeMovedByPlayer = true;

        if(Vector3.Distance(HeroManagerRef.CurrentTarget.transform.position, transform.position)  > HeroManagerRef.AttackRange + HeroManagerRef.CurrentTarget.GetComponent<BoxCollider>().size.z)
        {
            HeroManagerRef.MoveTo(HeroManagerRef.CurrentTarget.transform.position, HeroManagerRef.CurrentTarget.GetComponent<BoxCollider>().size.z + HeroManagerRef.AttackRange/2);
        }
        else if(timer <= 0f) 
        {
            Attack();
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

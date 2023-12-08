using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeroAIC;

public class HeroAIControlled : HeroBehaviour
{
    public override void ApplyBehaviour()
    {
        HeroManagerRef.canBeMovedByPlayer = true;
    }

    public override BehaviourName CheckTransition()
    {
        if (!Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()) && HeroManagerRef.CurrentTarget == null)
        {
            return BehaviourName.Wait;
        }
        else if (HeroManagerRef.CurrentTarget != null) 
        {
            return BehaviourName.Attack;
        }
        else return BehaviourName.None;
    }
}

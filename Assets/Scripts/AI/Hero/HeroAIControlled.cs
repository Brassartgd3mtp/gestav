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
        if (!Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()) && !HeroManagerRef.IsInBattle)
        {
            return BehaviourName.Wait;
        }
        else if (HeroManagerRef.IsInBattle) 
        {
            return BehaviourName.Attack;
        }
        else return BehaviourName.None;
    }
}
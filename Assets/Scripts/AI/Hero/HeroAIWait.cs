using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeroAIC;

public class HeroAIWait : HeroBehaviour
{
    public override void ApplyBehaviour()
    {
        HeroManagerRef.canBeMovedByPlayer = false;
    }

    public override BehaviourName CheckTransition()
    {
        if (Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<HeroManager>()))
        {
            return BehaviourName.Controlled;
        }
        else return BehaviourName.None;
    }
}

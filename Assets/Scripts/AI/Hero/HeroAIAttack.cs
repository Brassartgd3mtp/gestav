using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeroAIC;

public class HeroAIAttack : HeroBehaviour
{
    public override void ApplyBehaviour()
    {
        HeroManagerRef.canBeMovedByPlayer = true;
    }

    public override BehaviourName CheckTransition()
    {
        throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAIWait : WorkerBehaviour
{
    public override void ApplyBehaviour()
    {
     canBeMovedbyPlayer = false;
    }
    public override BehaviourName CheckTransition()
    {
    if(Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()))
        {
            return BehaviourName.Controlled;
        }
        if (CharacterManagerRef.isTransferingItems)
        {
            return BehaviourName.Transfer;
        }
        else return BehaviourName.None;
    }
}

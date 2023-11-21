using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAIControlled : WorkerBehaviour
{
    public override void ApplyBehaviour()
    {
        canBeMovedbyPlayer = true;
    }
    public override BehaviourName CheckTransition()
    {
        if(CharacterManagerRef.buildingAssigned != null) 
        {
            return BehaviourName.Use;
        }
        if(!Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()))
        {
            return BehaviourName.Wait;
        }
        if(CharacterManagerRef.isTransferingItems)
        {
            return BehaviourName.Transfer;
        }
    else return BehaviourName.None;
    }
}

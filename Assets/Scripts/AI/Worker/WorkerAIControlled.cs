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
        if(!Global.SELECTED_UNITS.Contains(this.gameObject.GetComponentInParent<UnitManager>()))
        {
            return BehaviourName.Wait;
        }
    else return BehaviourName.None;
    }
}

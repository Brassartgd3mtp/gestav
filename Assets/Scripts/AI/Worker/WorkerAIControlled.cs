using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAIControlled : WorkerBehaviour
{
    public override void ApplyBehaviour()
    {
        canBeMovedbyPlayer = true;
        currentActionText.text = "Controlled";
        currentActionText.color = Color.green;
        currentActionText.outlineColor = Color.black;
        currentActionText.outlineWidth = 0.35f;
    }
    public override BehaviourName CheckTransition()
    {
        if(CharacterManagerRef.isAssignedToABuilding) 
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

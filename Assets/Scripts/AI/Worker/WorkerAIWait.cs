using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAIWait : WorkerBehaviour
{
    public override void ApplyBehaviour()
    {
        canBeMovedbyPlayer = false;
        currentActionText.text = "Inactive";
        currentActionText.color = Color.red;
        currentActionText.outlineColor = Color.black;
        currentActionText.outlineWidth = 0.35f;
    }
    public override BehaviourName CheckTransition()
    {
    if(Global.SELECTED_CHARACTERS.Contains(this.gameObject.GetComponentInParent<CharacterManager>()))
        {
            return BehaviourName.Controlled;
        }
        if (WorkerManagerRef.isTransferingItems)
        {
            return BehaviourName.Transfer;
        }
        else return BehaviourName.None;
    }
}

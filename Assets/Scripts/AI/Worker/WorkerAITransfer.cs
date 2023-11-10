using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAITransfer : WorkerBehaviour
{
    public override void ApplyBehaviour()
    {
        canBeMovedbyPlayer = false;
    }
    public override BehaviourName CheckTransition()
    {
        throw new System.NotImplementedException();
    }
}

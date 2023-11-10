using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum BehaviourName
{
    None =1 <<0,
    Use = 1 <<1,
    Transfer=1<<2,
    Wait=1<<3,
    Controlled=1<<4
}

public class WorkerAIC : MonoBehaviour
{


    private WorkerBehaviour currentBehaviour;
    public WorkerBehaviour CurrentBehaviour => currentBehaviour;

    private WorkerAIUse workerAIUse;
    private WorkerAITransfer workerAITransfer;
    private WorkerAIWait workerAIWait;
    private WorkerAIControlled workerAIControlled;


    private void Awake()
    {
        workerAIUse = GetComponent<WorkerAIUse>();
        workerAITransfer = GetComponent<WorkerAITransfer>();
        workerAIWait = GetComponent<WorkerAIWait>();
        workerAIControlled = GetComponent<WorkerAIControlled>();

        currentBehaviour = workerAIWait;
    }
    private void Update()
    {
        currentBehaviour.ApplyBehaviour();

        // Vérifier la transition
        BehaviourName nextBehaviour = currentBehaviour.CheckTransition();

        // Changer de comportement si nécessaire
        if (nextBehaviour != BehaviourName.None)
        {
            ChangeBehaviour(nextBehaviour);
        }
    }

    public void ChangeBehaviour(BehaviourName behaviour)
    {
        currentBehaviour.enabled = false;

        switch (behaviour)
        {
            case BehaviourName.Use:
                currentBehaviour = workerAIUse;
                break;
            case BehaviourName.Transfer:
                currentBehaviour = workerAITransfer;
                break;
            case BehaviourName.Controlled:
                currentBehaviour = workerAIControlled;
                break;
            case BehaviourName.Wait:
                currentBehaviour = workerAIWait;
                break;
            default:
                break;
        }
        // Activer le nouveau script
        currentBehaviour.enabled = true;

        Debug.Log($"Switching to {behaviour} state.");

    }
}

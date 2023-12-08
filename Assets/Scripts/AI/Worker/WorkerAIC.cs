using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WorkerAIC : MonoBehaviour
{
    public enum BehaviourName
    {
        None,
        Use,
        Transfer,
        Wait,
        Controlled
    }

    private WorkerBehaviour currentBehaviour;
    public WorkerBehaviour CurrentBehaviour => currentBehaviour;

    private WorkerAIUse workerAIUse;
    private WorkerAITransfer workerAITransfer;
    private WorkerAIWait workerAIWait;
    private WorkerAIControlled workerAIControlled;

    private float updateTimer;
    private float updateInterval = 0.5f;


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

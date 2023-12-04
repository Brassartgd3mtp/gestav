using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static WorkerAIC;

public abstract class WorkerBehaviour : MonoBehaviour
{
    public WorkerManager WorkerManagerRef;
    public bool canBeMovedbyPlayer;
    public bool TransferStarted = false;
    protected GameResourceManager gameResourceManager;
    [SerializeField] protected TextMeshProUGUI currentActionText;
    public abstract void ApplyBehaviour();
    public abstract BehaviourName CheckTransition();


    protected virtual void Awake()
    {
        WorkerManagerRef = GetComponentInParent<WorkerManager>();
        gameResourceManager = FindObjectOfType<GameResourceManager>();
    }
}

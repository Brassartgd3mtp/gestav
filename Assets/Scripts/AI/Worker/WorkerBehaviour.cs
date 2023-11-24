using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static WorkerAIC;

public abstract class WorkerBehaviour : MonoBehaviour
{
    public CharacterManager CharacterManagerRef;
    public bool canBeMovedbyPlayer;
    public bool TransferStarted = false;
    protected GameResourceManager gameResourceManager;
    [SerializeField] protected TextMeshProUGUI currentActionText;
    public abstract void ApplyBehaviour();
    public abstract BehaviourName CheckTransition();


    protected virtual void Awake()
    {
        CharacterManagerRef = GetComponentInParent<CharacterManager>();
        gameResourceManager = FindObjectOfType<GameResourceManager>();
    }
}

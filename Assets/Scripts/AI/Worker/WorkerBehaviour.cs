using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public abstract class WorkerBehaviour : MonoBehaviour
{
    public CharacterManager CharacterManagerRef;
    public bool canBeMovedbyPlayer;
    public abstract void ApplyBehaviour();
    public abstract BehaviourName CheckTransition();


    protected virtual void Awake()
    {
        CharacterManagerRef = GetComponentInParent<CharacterManager>();
    }
}

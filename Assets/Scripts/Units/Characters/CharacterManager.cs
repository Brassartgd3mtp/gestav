using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CharacterManager : UnitManager
{
    private Character character;
    [SerializeField] private NavMeshAgent agent;

    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.destination = targetPosition;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAIC : MonoBehaviour
{
    public enum BehaviourName
    {
        None,
        Attack,
        Wait,
        Controlled
    }

    private HeroBehaviour currentBehaviour;
    public HeroBehaviour CurrentBehaviour => currentBehaviour;

    private HeroAIAttack heroAIattack;
    private HeroAIWait heroAIWait;
    private HeroAIControlled heroAIControlled;

    private float updateTimer;
    private float updateInterval = 0.5f;


    private void Awake()
    {
        heroAIattack = GetComponent<HeroAIAttack>();
        heroAIControlled = GetComponent<HeroAIControlled>();
        heroAIWait = GetComponent<HeroAIWait>();

        currentBehaviour = heroAIWait;
    }
    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer += Time.deltaTime;

            currentBehaviour.ApplyBehaviour();

            // Vérifier la transition
            BehaviourName nextBehaviour = currentBehaviour.CheckTransition();

            // Changer de comportement si nécessaire
            if (nextBehaviour != BehaviourName.None)
            {
                ChangeBehaviour(nextBehaviour);
            }
            updateTimer = 0f;
        }



    }

    public void ChangeBehaviour(BehaviourName behaviour)
    {
        currentBehaviour.enabled = false;

        switch (behaviour)
        {
            case BehaviourName.Attack:
                currentBehaviour = heroAIattack;
                break;
            case BehaviourName.Controlled:
                currentBehaviour = heroAIControlled;
                break;
            case BehaviourName.Wait:
                currentBehaviour = heroAIWait;
                break;
            default:
                break;
        }
        // Activer le nouveau script
        currentBehaviour.enabled = true;

        Debug.Log($"Switching to {behaviour} state.");

    }
}


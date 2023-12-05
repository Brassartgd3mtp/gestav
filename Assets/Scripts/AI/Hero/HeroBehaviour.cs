using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HeroAIC;

public abstract class HeroBehaviour : MonoBehaviour
{
    public HeroManager HeroManagerRef;
    public abstract void ApplyBehaviour();
    public abstract BehaviourName CheckTransition();


    protected virtual void Awake()
    {
        HeroManagerRef = GetComponentInParent<HeroManager>();
    }
}

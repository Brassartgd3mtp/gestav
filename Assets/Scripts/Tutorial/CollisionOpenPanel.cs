using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionOpenPanel : MonoBehaviour
{
    private Tutorial tutorialScript;
    private void Awake()
    {
        tutorialScript = FindAnyObjectByType<Tutorial>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CharacterManager characterManager))
        {
            if(tutorialScript.tutorialStep == 2)
            {
                tutorialScript.panelStepTwo.SetActive(true);
            }
            if(tutorialScript.tutorialStep ==3) 
            {
                tutorialScript.panelStepThree.SetActive(true);
            }
        }
    }
}

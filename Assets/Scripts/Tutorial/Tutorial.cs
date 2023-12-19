using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int tutorialStep = 1;

    [SerializeField] private GameObject panelStepOne;
    public GameObject panelStepTwo;
    public GameObject panelStepThree;

    private void Awake()
    {
        panelStepOne.SetActive(true);
        Time.timeScale = 0f;
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(tutorialStep == 1 && panelStepOne.activeSelf ) 
            {
                panelStepOne.SetActive(false);
                tutorialStep++;
                Time.timeScale = 1f;
            }
            else if(tutorialStep == 2 && panelStepTwo.activeSelf )
            {
                panelStepTwo.SetActive(false);
                tutorialStep++;
                Time.timeScale = 1f;
            }
            else if (tutorialStep == 3 && panelStepThree.activeSelf )
            {
                panelStepThree.SetActive(false);
                tutorialStep++;
                Time.timeScale = 1f;
            }
        }
    }
}

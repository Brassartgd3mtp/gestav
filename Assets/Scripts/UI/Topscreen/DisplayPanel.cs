using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelObject;

    private void Awake()
    {
        panelObject.SetActive(false);
    }

    public void TogglePanel()
    {
        if(panelObject.activeSelf) 
        {
        panelObject.SetActive(false);
        }
        else
        {
            panelObject.SetActive(true);
        }
    }
}

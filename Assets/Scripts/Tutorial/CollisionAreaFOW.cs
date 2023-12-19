using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollisionAreaFOW : MonoBehaviour
{
    public TextMeshProUGUI FogText;

    private void Awake()
    {
        FogText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CharacterManager characterManager)) 
        {
        FogText.gameObject.SetActive(true);
        }
    }

}

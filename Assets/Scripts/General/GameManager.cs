using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DataHandler.LoadGameData();
        Debug.Log("Game Data Loaded");
    }
}

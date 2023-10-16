using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    private ResourceSource resourceSource { get; set; }
    private int currentAmount = 0;

    public UnityEvent OnQuantityChange;

    private void Start()
    {
        currentAmount = resourceSource.MaxResources;
        Debug.Log(currentAmount);
    }


    public void GatherResource(int _quantity)
    {
        currentAmount -= _quantity;
        int amountToGive = _quantity;
        Debug.Log(currentAmount);

        //give the right amount of resources if the resource source is at 0 resources remaining
        if (currentAmount < 0)
        {
            amountToGive = _quantity + currentAmount;
        }
        if (currentAmount <= 0)
        {
            Destroy(gameObject);
        }

        if(OnQuantityChange != null)
        {
            OnQuantityChange.Invoke();
        }

    }


}

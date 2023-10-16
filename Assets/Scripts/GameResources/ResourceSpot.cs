using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ResourceType
{
    Copper
}

public class ResourceSpot : MonoBehaviour
{
    [SerializeField] private ResourceType Type;
    [SerializeField] private int quantity;
    [SerializeField] GameObject parentObject;

    public UnityEvent onQuantityChange;

        

    public void GatherResources(int _quantity)
    {

        quantity -= _quantity;
        int amountToGive = _quantity;
        Debug.Log(quantity);

        //give the right amount of resources if the resource source is at 0 resources remaining
        if (quantity < 0)
        {
            amountToGive = _quantity + quantity;
        }
        if (quantity <= 0)
        {
            Destroy(parentObject);
        }

        if (onQuantityChange != null)
        {
            onQuantityChange.Invoke();
        }

    }

}

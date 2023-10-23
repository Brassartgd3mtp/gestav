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
    public int Quantity;
    [SerializeField] GameObject parentObject;

    public UnityEvent onQuantityChange;
    private ResourceSpotUI resourceSpotUI;

    private void Awake()
    {
        resourceSpotUI = parentObject.GetComponent<ResourceSpotUI>();
        resourceSpotUI.UpdateResourceQuantity();
    }


    public void GatherResources(int _quantity)
    {

        Quantity -= _quantity;
        resourceSpotUI.UpdateResourceQuantity();

        int amountToGive = _quantity;
        Debug.Log(Quantity);

        //give the right amount of resources if the resource source is at 0 resources remaining
        if (Quantity < 0)
        {
            amountToGive = _quantity + Quantity;
        }
        if (Quantity <= 0)
        {
            Destroy(parentObject);
            Global.RebuildNavMesh();
        }

        if (onQuantityChange != null)
        {
            onQuantityChange.Invoke();
        }

    }

}

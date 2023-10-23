using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceSpotUI : MonoBehaviour
{
    public GameObject PopUpPanel;
    public TextMeshProUGUI QuantityText;
    public ResourceSpot Resource;

    private void Awake()
    {
        PopUpPanel.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        PopUpPanel.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        PopUpPanel.gameObject.SetActive(false);
    }

    public void UpdateResourceQuantity()
    {
        QuantityText.text = Resource.Quantity.ToString();
    }
}

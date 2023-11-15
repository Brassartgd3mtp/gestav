using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NumberInput : MonoBehaviour
{
    private TMP_InputField numberInputField;
    private int result;
    public int Result => result;

    public string userInput;

    private void Awake()
    {
        numberInputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        // Subscribe to the onValueChanged event to handle changes in the input field
        numberInputField.onValueChanged.AddListener(OnNumberValueChanged);
    }

    private void OnNumberValueChanged(string newValue)
    {
        // This function is called whenever the value in the input field changes
        // You can convert the string to a numeric type (e.g., int or float) and use it as needed
        if (int.TryParse(newValue, out int _result))
        {
            // Use 'result' as the numeric value entered by the user
            Debug.Log("Entered number: " + _result);
            result = _result;
        }
        else
        {
            // Handle invalid input (non-numeric)
            Debug.LogWarning("Invalid input. Please enter a number.");
        }
    }

    public void DecreaseByOne()
    {
        result--;
        numberInputField.text = result.ToString();
    }
    public void IncreaseByOne()
    {
        result++;
        numberInputField.text = result.ToString();
    }
}

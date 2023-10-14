using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a class named GameResources
public class GameResources
{
    // Private member variables to store the resource's name and current amount
    private string name;
    private int currentAmount;

    // Constructor for GameResources, initializing name and currentAmount
    public GameResources(string _name, int _initialAmount)
    {
        name = _name;
        currentAmount = _initialAmount;
    }

    // Method to add an amount to the currentAmount
    public void AddAmount(int _value)
    {
        currentAmount += _value;

        // Ensure that the currentAmount is never negative
        if (currentAmount < 0)
            currentAmount = 0;
    }

    // Property to access the name of the resource
    public string Name { get => name; }

    // Property to access the current amount of the resource
    public int Amount { get => currentAmount; }
}

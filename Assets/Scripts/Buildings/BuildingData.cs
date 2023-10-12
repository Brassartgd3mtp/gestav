using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData
{
    private string code; // the unique code of the building (that will be used to get the proper prefab
    private int healthPoints; //amount of healthpoints the instances of this reference will have when created

    public BuildingData(string _code, int _healthPoints)
    {
        code = _code;
        healthPoints = _healthPoints;
    }

    public string Code { get => code; }
    public int HP { get => healthPoints; }
}

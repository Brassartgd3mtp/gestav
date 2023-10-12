using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{

    public static int TERRAIN_LAYER_MASK = 1 << 9; // the second number is the number index of our "Terrain" Layer

    public static BuildingData[] BUILDING_DATA = new BuildingData[]
    {
        new BuildingData("Mine", 20),
        new BuildingData("Forge", 30),
        new BuildingData("Workshop", 40)
    };
}

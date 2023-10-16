using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{

    public static int TERRAIN_LAYER_MASK = 1 << 9; // the second number is the number index of our "Terrain" Layer
    public static int RESOURCE_LAYER_MASK = 1 << 13;

    public static BuildingData[] BUILDING_DATA = new BuildingData[0];

    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();


}

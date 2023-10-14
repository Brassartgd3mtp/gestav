using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{

    public static int TERRAIN_LAYER_MASK = 1 << 9; // the second number is the number index of our "Terrain" Layer


    public static BuildingData[] BUILDING_DATA;

    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();
}

using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Global
{

    public static int TERRAIN_LAYER_MASK = 1 << 9; // the second number is the number index of our "Terrain" Layer
    public static int RESOURCE_LAYER_MASK = 1 << 13;
    public static int MINE_LAYER_MASK = 1 << 14;
    public static int FORGE_LAYER_MASK = 1 << 15;
    public static int WORKSHOP_LAYER_MASK = 1 << 16;
    public static int WORKER_LAYER_MASK = 1 << 12; 
    public static int BUILDING_LAYER_MASK = (1 << 10) | (1 << 14) | (1<<15) | (1<<16);
    public static int UNIT_LAYER_MASK = (1<<10) | (1 << 14) | (1 << 15) | (1 << 16) | (1 << 12);

    public static BuildingData[] BUILDING_DATA = new BuildingData[0];

    public static List<CharacterManager> SELECTED_CHARACTERS = new List<CharacterManager>();
    public static List<BuildingManager> SELECTED_BUILDINGS = new List<BuildingManager>();
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static List<BuildingInventory> BUILDINGS = new List<BuildingInventory>();

    public static List<UnitInventory> allInventories = new List<UnitInventory>();
    public static List<InventoryItemData> allItems = new List<InventoryItemData>();
    public static List<ItemTypeAndCount> TotalItemsTypeAndCount = new List<ItemTypeAndCount>();

    public static void RebuildNavMesh()
    {
        GameObject Ground = GameObject.Find("Ground");
        Ground.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

}

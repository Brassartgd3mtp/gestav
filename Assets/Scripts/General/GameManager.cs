using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GameManager : MonoBehaviour
{

    private Ray ray;
    private RaycastHit raycastHit;

    private void Awake()
    {
        DataHandler.LoadGameData();
        Debug.Log("Game Data Loaded");
    }

    private void Update()
    {
        CheckUnitsNavigation();
    }
    private void CheckUnitsNavigation()
    {
        if (Global.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                ray,
                out raycastHit,
                1000f,
                Global.TERRAIN_LAYER_MASK
            ))
            {
                foreach (UnitManager um in Global.SELECTED_UNITS)
                    if (um.GetType() == typeof(CharacterManager))
                    {
                        ((CharacterManager)um).MoveTo(raycastHit.point);
                        ((CharacterManager)um).ExitGatheringMode();
                    }
            }


            if (Global.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(
                    ray,
                    out raycastHit,
                    1000f,
                    Global.RESOURCE_LAYER_MASK
                ))
                {
                    foreach (UnitManager um in Global.SELECTED_UNITS)
                        if (um.GetType() == typeof(CharacterManager))
                        {
                            ((CharacterManager)um).MoveTo(raycastHit.point);
                            ((CharacterManager)um).EnterGatheringMode();
                        }

                }

            }

        }
    }
}

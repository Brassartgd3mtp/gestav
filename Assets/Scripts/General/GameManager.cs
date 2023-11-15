using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.Burst.CompilerServices;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject groundMarker;
    private bool isActive;

    private Ray ray;
    private RaycastHit raycastHit;

    public bool IsInteracting {  get; private set; }
    private void Awake()
    {
        DataHandler.LoadGameData();
        Debug.Log("Game Data Loaded");
    }

    private void Update()
    {
        CheckUnitsNavigation();
        CheckUnitInteractedWith();
    }
    private void CheckUnitsNavigation()
    {
        if (Global.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            foreach(UnitManager unit in Global.SELECTED_UNITS) 
            {
            WorkerAIC aic = unit.gameObject.GetComponentInChildren<WorkerAIC>();
                if (aic.CurrentBehaviour.canBeMovedbyPlayer == true)
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                    if (Physics.Raycast(ray, out raycastHit, 1000f, Global.TERRAIN_LAYER_MASK) && !isOverUI)
                    {

                        groundMarker.transform.position = raycastHit.point;
                        groundMarker.SetActive(true);

                        // Active le timer en passant isActive � true
                        isActive = true;
                        Invoke("DisableGroundMarker", 2.0f); // Appelle la m�thode DisableGroundMarker apr�s 2 secondes


                        foreach (UnitManager um in Global.SELECTED_UNITS)
                            if (um.GetType() == typeof(CharacterManager))
                            {
                                ((CharacterManager)um).MoveTo(raycastHit.point, 0f);
                                ((CharacterManager)um).ExitGatheringMode();
                            }
                    }
            }

            }


            if (Global.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (Physics.Raycast(
                    ray,
                    out raycastHit,
                    1000f,
                    Global.RESOURCE_LAYER_MASK
                ) && !isOverUI)
                {
                    foreach (UnitManager um in Global.SELECTED_UNITS)
                        if (um.GetType() == typeof(CharacterManager))
                        {
                            ((CharacterManager)um).MoveTo(raycastHit.point, 0f);
                            ((CharacterManager)um).EnterGatheringMode();
                        }

                }

            }
        }
    }


    void DisableGroundMarker()
    {
        // D�sactive le marqueur au bout de 2 secondes
        groundMarker.SetActive(false);

        // D�sactive le timer en passant isActive � false
        isActive = false;
    }


    public void CheckUnitInteractedWith()
    {
        if (Global.SELECTED_UNITS.Count == 1 && Input.GetMouseButtonUp(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            if (Physics.Raycast(
                ray,
                out raycastHit,
                1000f,
                Global.UNIT_LAYER_MASK) && !isOverUI
                
            )
            {
                GameObject hitObject = raycastHit.collider.gameObject;
                var interactable = hitObject.GetComponent<IInteractable>();
                if (interactable != null) StartInteraction(interactable);
            }
        }
        else EndInteraction(); 
    }

    public void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactionWasSuccessful);
        IsInteracting = true;
    }

    public void EndInteraction()
    {
        IsInteracting = false;
    }

}

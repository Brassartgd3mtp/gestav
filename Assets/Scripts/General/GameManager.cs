using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject groundMarker;

    private Ray ray;
    private RaycastHit raycastHit;

    public bool IsInteracting {  get; private set; }
    private void Awake()
    {
        DataHandler.LoadGameData();
    }

    private void Update()
    {
        CheckUnitsNavigation();
        CheckUnitInteractedWith();
    }
    private void CheckUnitsNavigation()
    {
        if (Global.SELECTED_CHARACTERS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            foreach (UnitManager unit in Global.SELECTED_CHARACTERS)
            {
                WorkerAIC aic = unit.gameObject.GetComponentInChildren<WorkerAIC>();

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (Physics.Raycast(ray, out raycastHit, 1000f, Global.TERRAIN_LAYER_MASK) && !isOverUI)
                {

                    groundMarker.transform.position = raycastHit.point;
                    groundMarker.SetActive(true);

                    // Active le timer en passant isActive à true
                    Invoke("DisableGroundMarker", 2.0f); // Appelle la méthode DisableGroundMarker après 2 secondes


                    foreach (UnitManager um in Global.SELECTED_CHARACTERS)
                        if (um is CharacterManager)
                        {
                         WorkerManager wm = um as WorkerManager;
                         HeroManager hm = um as HeroManager;
                            if (wm!= null && wm.canBeMovedByPlayer)
                            {
                                ((WorkerManager)um).MoveTo(raycastHit.point, 1f);
                                ((WorkerManager)um).ExitGatheringMode();
                            }
                         
                            if(hm != null && hm.canBeMovedByPlayer)
                            {
                                ((HeroManager)um).MoveTo(raycastHit.point, 1f);
                                hm.CurrentTarget = null;
                            }
                            
                        }
                }
            }
        }
    }
    


    void DisableGroundMarker()
    {
        // Désactive le marqueur au bout de 2 secondes
        groundMarker.SetActive(false);

        // Désactive le timer en passant isActive à false
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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR;

public class CharacterManager : UnitManager
{
    [Header("Scripts")]

    [SerializeField] protected Find findingScript;
    public UnitData unitData;
    protected Character character;

    protected ResourceSpot resourceSpot;
    public ResourceSpot ResourceSpot => resourceSpot;
    protected ItemRef item;
    public ItemRef Item => item;


    [Header("Navigation")]

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float stoppingDistance;
    protected Vector3 targetPosition;

    [Header("Animations & graphics")]

    [SerializeField] protected Animator animator;
    public Animator Animator => animator;
    [SerializeField] protected GameObject corpse;

    [SerializeField] private Transform meshTrasnform;


    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    protected virtual void Awake()
    {
        healthBar.maxValue = unitData.healthPoints;
        HealthPoints = unitData.healthPoints;
    }

    public override void HealthUpdate()
    {
        base.HealthUpdate();

        if (HealthPoints <= 0)
        {
            Instantiate(corpse, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
            
    }
    public async void MoveTo(Vector3 _targetPosition, float _rangeToStop)
    {
        bool positionReached = false;
        meshTrasnform.rotation = transform.rotation;
        // Stop the current movement
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        // Set the new destination
        agent.destination = _targetPosition;
        targetPosition = _targetPosition;
        // Resume movement
        agent.isStopped = false;
        animator.SetBool("Walking", true);

        transform.LookAt(targetPosition);
    
        while (!positionReached)
        {
            await Task.Delay(100);

            if (agent.velocity == Vector3.zero || Vector3.Distance(transform.position, _targetPosition) < _rangeToStop)
            {
                animator.SetBool("Walking", false);
                agent.isStopped = true;
                positionReached = true;
                return;
            }

        }


    }



    // Utility method for selecting the unit
    protected override void SelectUtil()
    {
        base.SelectUtil();
        if (Global.SELECTED_CHARACTERS.Contains(this)) return;
        Global.SELECTED_CHARACTERS.Add(this);

        AddMaterial(OutilineMaterial);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public override void Select()
    {
        Select(false, false);
    }

    public override void Select(bool _singleClick, bool _holdingShift)
    {
        base.Select();
        // Basic case: using the selection box
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<CharacterManager> selectedUnits = new List<CharacterManager>(Global.SELECTED_CHARACTERS);
            foreach (CharacterManager um in selectedUnits)

                um.Deselect();
            SelectUtil();


        }
        else
        {
            if (!Global.SELECTED_CHARACTERS.Contains(this))
                SelectUtil();
            else
                Deselect();
        }
    }

    // Deselect the unit
    public override void Deselect()
    {
        base.Deselect();
        if (!Global.SELECTED_CHARACTERS.Contains(this)) return;
        Global.SELECTED_CHARACTERS.Remove(this);

        RemoveMaterial("M_Outline (Instance)");
    }


    public override void AddMaterial(Material material)
    {
        SkinnedMeshRenderer meshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            // Récupère les matériaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Ajoute le nouveau matériau à la liste des matériaux
            materialList.Add(material);

            // Applique la nouvelle liste de matériaux au MeshRenderer
            meshRenderer.materials = materialList.ToArray();
        }


    }

    public override void RemoveMaterial(string materialName)
    {
        SkinnedMeshRenderer meshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            // Récupère les matériaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Recherche et enlève le matériau spécifié de la liste par nom
            Material materialToRemove = materialList.Find(m => m.name == materialName);

            if (materialToRemove != null)
            {
                materialList.Remove(materialToRemove);

                // Applique la nouvelle liste de matériaux au MeshRenderer
                meshRenderer.materials = materialList.ToArray();
            }
        }
    }
}


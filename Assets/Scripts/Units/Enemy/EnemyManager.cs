using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
public class EnemyManager : MonoBehaviour
{
    [Header("Statistics")]
    public int HealthPoints;
    public int Attack;
    public float AttackSpeed;

    private BoxCollider _collider;
    public EnemyData EnemyData;

    [Header("Navigation")]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance;
    protected Vector3 targetPosition;

    [Header("Animations & graphics")]

    [SerializeField] private Animator animator;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject corpse;
    [SerializeField] private Material outlineMaterial;

    private void Awake()
    {
        healthBar.maxValue = EnemyData.healthPoints;
        HealthPoints = EnemyData.healthPoints;
        Attack = EnemyData.damages;
        AttackSpeed = EnemyData.attackSpeed;
    }

    protected void FixedUpdate()
    {
        HealthUpdate();
    }
    public void HealthUpdate()
    {
        healthBar.value = HealthPoints;
        if (HealthPoints <= 0)
        {
            Instantiate(corpse, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        AddMaterial(outlineMaterial);
    }
    private void OnMouseExit()
    {
        RemoveMaterial("M_Outline_Enemy (Instance)");
    }

    private void InflictDamage(UnitManager target, int damage)
    {
        damage = Attack;
        target.HealthPoints -= damage;
        target.HealthUpdate();
    }

    public async void MoveTo(Vector3 _targetPosition, float _rangeToStop)
    {
        bool positionReached = false;

        // Stop the current movement
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        Debug.Log(animator.GetBool("Walking"));
        // Set the new destination
        agent.destination = _targetPosition;
        targetPosition = _targetPosition;
        // Resume movement
        agent.isStopped = false;
        animator.SetBool("Walking", true);
        Debug.Log(animator.GetBool("Walking"));

        //  Vector3 dir = transform.position - targetPosition;
        // transform.rotation = Quaternion.LookRotation(dir);

        while (!positionReached)
        {
            await Task.Delay(100);

            if (agent.velocity == Vector3.zero)
            {
                animator.SetBool("Walking", false);
                Debug.Log(animator.GetBool("Walking"));
                agent.isStopped = true;
                positionReached = true;
                return;
            }

        }


    }
    public void AddMaterial(Material material)
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // R�cup�re les mat�riaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Ajoute le nouveau mat�riau � la liste des mat�riaux
            materialList.Add(material);

            // Applique la nouvelle liste de mat�riaux au MeshRenderer
            meshRenderer.materials = materialList.ToArray();
        }
    }

    public void RemoveMaterial(string materialName)
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // R�cup�re les mat�riaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Recherche et enl�ve le mat�riau sp�cifi� de la liste par nom
            Material materialToRemove = materialList.Find(m => m.name == materialName);

            if (materialToRemove != null)
            {
                materialList.Remove(materialToRemove);

                // Applique la nouvelle liste de mat�riaux au MeshRenderer
                meshRenderer.materials = materialList.ToArray();
            }
        }
    }
}

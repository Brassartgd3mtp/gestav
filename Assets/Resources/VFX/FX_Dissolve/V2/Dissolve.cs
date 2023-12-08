using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 0.75f;
    [SerializeField] private float startDissolveAmount;
    [SerializeField] private float endDissolveAmount;

    private Renderer[] renderers;
    private Material[] materials;

    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private GameObject transparencyMine;


    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        buildingManager = GetComponentInParent<BuildingManager>();
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }
    }

    private void Update()
    {
        if (buildingManager != null && buildingManager.hasBeenBuilt)
        {
            StartCoroutine(Appear(true, false));
            transparencyMine.SetActive(false);
        }
    }
    private IEnumerator Vanish(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(startDissolveAmount, endDissolveAmount, (elapsedTime / dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(startDissolveAmount, endDissolveAmount, (elapsedTime / dissolveTime));

            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(dissolveAmount, lerpedDissolve);

                if (useVertical)
                    materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
            }

            yield return null;
        }
    }

    private IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / dissolveTime));

            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(dissolveAmount, lerpedDissolve);

                if (useVertical)
                    materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
            }

            yield return null;
        }
    }
}

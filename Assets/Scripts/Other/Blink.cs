using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public float blinkSpeed = 0.5f;  // Adjust the speed of the blink
    private MeshRenderer meshRenderer;
    private bool isBlinking = false;
    private bool blinkState = false;
    private Color originalColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    public void StartBlinking()
    {
        isBlinking = true;
        StartCoroutine(BlinkObject());
    }

    public void StopBlinking()
    {
        StopCoroutine(BlinkObject());
        isBlinking = false;
        meshRenderer.material.color = originalColor;
    }

    IEnumerator BlinkObject()
    {
        while (isBlinking)
        {
            blinkState = !blinkState;

            // Use Lerp to smoothly transition between original color and white
            Color targetColor = blinkState ? Color.white : originalColor;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / blinkSpeed;
                meshRenderer.material.color = Color.Lerp(GetComponent<MeshRenderer>().material.color, targetColor, t);
                yield return null;
            }

            if(!isBlinking)
            {
                meshRenderer.material.color = originalColor;
            }
            // Wait for a short duration before starting the next blink
            yield return new WaitForSeconds(0.1f);
        }
    }
}

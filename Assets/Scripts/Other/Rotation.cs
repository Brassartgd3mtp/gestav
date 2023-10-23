using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationSpeed = 5f;

    void Update()
    {
        // Fait tourner l'objet autour de l'axe Y à la vitesse spécifiée
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl
{
    public class CameraRotation : MonoBehaviour
    {
    [SerializeField] private float smoothingFactor = 5f; // Smoothing factor for rotation
    [SerializeField] private float distanceToStop = 0.5f;

        private bool isRotating = false;
        private Quaternion targetRotation;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
            {
                targetRotation = transform.rotation * Quaternion.Euler(0, -45, 0);
                StartRotation();
            }
            else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
            {
                targetRotation = transform.rotation * Quaternion.Euler(0, 45, 0);
                StartRotation();
            }

            if (isRotating)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothingFactor * Time.deltaTime);

                // Check if we're close enough to the target rotation
                if (Quaternion.Angle(transform.rotation, targetRotation) < distanceToStop)
                {
                    StopRotation();
                }
            }
        }

        private void StartRotation()
        {
            isRotating = true;
        }

        private void StopRotation()
        {
            isRotating = false;
        }
    }
}

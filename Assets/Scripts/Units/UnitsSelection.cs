using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsSelection : MonoBehaviour
{
    private bool isDraggingMouseBox = false;
    private Vector3 dragStartPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraggingMouseBox = true;
            dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            isDraggingMouseBox = false;
    }

    void OnGUI()
    {
        if (isDraggingMouseBox)
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect(dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }
}

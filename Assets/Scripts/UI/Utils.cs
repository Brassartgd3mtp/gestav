using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // Static texture for a white 1x1 pixel, used for drawing shapes
    static Texture2D whiteTexture;

    public static Texture2D WhiteTexture
    {
        get
        {
            if (whiteTexture == null)
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            return whiteTexture;
        }
    }

    // Draw a colored rectangle on the screen
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    // Draw a colored border around a rectangle
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Draw top border
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Draw left border
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Draw right border
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Draw bottom border
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    // Convert screen positions to a screen rectangle
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    // Convert screen positions to a bounds object in the camera's viewport
    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Convert screen positions to viewport positions
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        // Find the minimum and maximum values in the viewport
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        // Set the depth values to match the camera's clipping planes
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        // Create and return a bounds object
        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}

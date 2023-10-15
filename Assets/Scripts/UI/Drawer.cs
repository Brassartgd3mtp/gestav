using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawerController : MonoBehaviour
{
    public RectTransform drawerPanel;
    public Button toggleButton;
    public RectTransform rectToggleButton;

    private bool isDrawerOpen = false;
    private float drawerHeight;
    private float buttonHeight;
    private float buttonWidth;

    void Start()
    {
        drawerHeight = drawerPanel.rect.height;

        buttonHeight = rectToggleButton.rect.height;
        buttonWidth = rectToggleButton.rect.width;
        toggleButton.onClick.AddListener(ToggleDrawer);
        CloseDrawer();
    }

    void ToggleDrawer()
    {
        if (isDrawerOpen)
            CloseDrawer();
        else
            OpenDrawer();
    }

    void OpenDrawer()
    {
        drawerPanel.anchoredPosition = new Vector2(0, 0.5f * drawerHeight);
        rectToggleButton.anchoredPosition = new Vector2(0 + 0.5f * buttonWidth, drawerHeight + 0.5f * buttonHeight);
        isDrawerOpen = true;
    }

    void CloseDrawer()
    {
        drawerPanel.anchoredPosition = new Vector2(0,0.5f * -drawerHeight);
        rectToggleButton.anchoredPosition = new Vector2(0 + 0.5f * buttonWidth,0 + 0.5f * buttonHeight);
        isDrawerOpen = false;
    }
}
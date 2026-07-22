using System.Collections.Generic;
using UnityEngine;

public class UITEST : MonoBehaviour
{
    public List<RectTransform> gamePanels; //0: Pause, 1: Win, 2: Lose

    private RectTransform currentPanel;

    public void OnButtonClick()
    {
        MovePanel();
    }


    public void MovePanel()
    {
        gamePanels[0].anchoredPosition = Vector3.zero;
        currentPanel = gamePanels[0];
    }
}

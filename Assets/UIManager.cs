using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    public void ActivateWinPanel()
    {
        winPanel.SetActive(true); 
    }
    public void ActivateLosePanel()
    {
        losePanel.SetActive(true);
    }

}

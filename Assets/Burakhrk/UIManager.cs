using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
    public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] Text levelText;
    public void ActivateWinPanel()
    {
        winPanel.SetActive(true); 
    }
    public void ActivateLosePanel()
    {
        losePanel.SetActive(true);
    }
    public void SetLevelText(int levelNo)
    {
        levelText.text = ("Level " + levelNo);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
    public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] Text levelText;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;

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
    public void SetHealth(int health, float healthMultiplier)
    {
      //  DOVirtual.Int(healthBar.fillAmount, health * healthMultiplier, 1f);


        healthBar.fillAmount = health * healthMultiplier;
        healthText.text = health*healthMultiplier + " / " + "100";
    }
}

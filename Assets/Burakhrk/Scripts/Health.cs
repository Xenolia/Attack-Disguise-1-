using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.NiceVibrations;
public class Health : MonoBehaviour
{
      [SerializeField] float totalHealth;
   [SerializeField] float healthMultiplier;
    CombatScript combatScript;

    UIManager uiManager;
    [SerializeField] float takeDamageDuration=1;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
       // uiManager = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<UIManager>();
        combatScript = GetComponent<CombatScript>();
        healthMultiplier = 1 / totalHealth;
    }
     
    public void TakeDamage(int Damage)
    {
        totalHealth = (totalHealth - Damage);
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
         if (totalHealth <= 0)
        {
            combatScript.Dead();
            totalHealth = 0;
        }
        //  uiManager.SetHealth(totalHealth, healthMultiplier);
        DOTween.To(() => healthBar.fillAmount, x => healthBar.fillAmount = x, healthMultiplier * totalHealth, takeDamageDuration).SetEase(Ease.OutBounce);
      //  healthBar.fillAmount = healthMultiplier * totalHealth;
        healthText.text = totalHealth * healthMultiplier * 100 + " / " + "100";
    }


}

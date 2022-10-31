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
    BattleManager battleManager;
    UIManager uiManager;
    [SerializeField] float takeDamageDuration=1;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
       // uiManager = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<UIManager>();
        combatScript = GetComponent<CombatScript>();
        healthMultiplier = 1 / totalHealth;
        HideHealth();
    }
    private void OnEnable()
    {
        battleManager.OnBattle += ShowHealth;
        battleManager.OnBattleFinished += HideHealth;
    }
    private void OnDisable()
    {
        battleManager.OnBattle -= ShowHealth;
        battleManager.OnBattleFinished -= HideHealth;

    }
    public void ShowHealth()
    {
        healthBar.transform.parent.gameObject.SetActive(true);
      //  healthBar.enabled = true;
      // healthText.enabled = true;
    }
    public void HideHealth()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
        /*
        healthBar.enabled = false;
        healthText.enabled = false;
        */
    }

    public void TakeDamage(int Damage)
    {
        ShowHealth();
        totalHealth = (totalHealth - Damage);
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
         if (totalHealth <= 0)
        {
            combatScript.Dead();
            totalHealth = 0;
        }
        //  uiManager.SetHealth(totalHealth, healthMultiplier);
        DOTween.To(() => healthBar.fillAmount, x => healthBar.fillAmount = x, healthMultiplier * totalHealth, takeDamageDuration).SetEase(Ease.InOutQuart);
      //  healthBar.fillAmount = healthMultiplier * totalHealth;
      if(healthText)
        healthText.text = totalHealth * healthMultiplier * 100 + " / " + "100";
    }


}

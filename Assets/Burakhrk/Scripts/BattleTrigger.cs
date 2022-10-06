using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleTrigger : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    [SerializeField] AttackButtonController abcPrefab;
    [SerializeField] EnemyDetection enemyDetection;
    [SerializeField] Canvas canvas;
    [SerializeField] EnemyScript[] enemyScript; 
    private void Awake()
    {
        if (!battleManager)
            battleManager = GameManager.Instance.gameObject.GetComponent<BattleManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            GetComponent<Collider>().enabled = false;
            other.gameObject.GetComponent<CombatScript>().enabled = true;
            battleManager.OnBattleInvoke();
            CreateButtons();
        }
    }


    void CreateButtons()
    {
        foreach (var enemy in enemyScript)
        {
            var abc = Instantiate(abcPrefab, canvas.transform) as AttackButtonController;
            abc.Init(enemy, enemyDetection);
        }
    }
}

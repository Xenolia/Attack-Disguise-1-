using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleTrigger : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    [SerializeField] GameObject abcPrefab;
    [SerializeField] EnemyDetection enemyDetection;
    [SerializeField] Canvas canvas;
    [SerializeField] EnemyManager enemyManager;
    EnemyScript[] enemyScript; 
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
        canvas = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<Canvas>();
        enemyScript = enemyManager.enemies;
        foreach (var enemy in enemyScript)
        {
            var abc = Instantiate(abcPrefab, canvas.transform);
            abc.GetComponentInChildren<AttackButtonController>().Init(enemy, enemyDetection);
        }
    }
}

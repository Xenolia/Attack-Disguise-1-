using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    private void Awake()
    {
        if (!battleManager)
            battleManager = GameManager.Instance.gameObject.GetComponent<BattleManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
          other.gameObject.GetComponent<CombatScript>().enabled = true;
           battleManager.OnBattleInvoke();
        }
    }
}

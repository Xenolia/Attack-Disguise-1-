using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           other.gameObject.GetComponent<CombatScript>().enabled = true;
            battleManager.OnBattleInvoke();
        }
    }
}

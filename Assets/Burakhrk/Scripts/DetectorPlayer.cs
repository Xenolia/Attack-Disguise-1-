using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorPlayer : MonoBehaviour
{
    Watcher target;
    float counter=1.5f;
    AutoAttack autoAttack;
    private void Awake()
    {
        autoAttack = GetComponentInParent<AutoAttack>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
         
            Watcher watcher= other.gameObject.GetComponent<Watcher>();
            if (!watcher)
                return;
            if(other.gameObject.GetComponent<Watcher>().isDead==false&&watcher.enabled)
            {
                Debug.Log("Enemy Detected");
                target = other.GetComponent<Watcher>();
                autoAttack.AttackCheck(target);

            }
        }
    }
}

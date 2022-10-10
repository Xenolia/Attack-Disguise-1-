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
                Debug.LogError("Enemy Detected");
                target = other.GetComponent<Watcher>();
                autoAttack.AttackCheck(target);

            }
        }
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        if (target)
            return;

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<Watcher>())
        {
            if (other.gameObject.GetComponent<Watcher>().isDead == false)
            {
                Debug.LogError("Enemy Detected");
                target = other.GetComponent<Watcher>();
                GetComponentInParent<AutoAttack>().AttackCheck(target);

            }
        }
    }
    */
    /*
    private void Update()
    {
        if (!target)
            return;

        if (counter > 0)
            counter = counter - Time.deltaTime;

        if (counter < 0)
        {
            target = null;
            counter = 1.5f;
        }
    }
    */

}

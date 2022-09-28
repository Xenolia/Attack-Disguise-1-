using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorPlayer : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")&& other.gameObject.GetComponent<Watcher>())
        {
            if(other.gameObject.GetComponent<Watcher>().isDead==false)
            {
                Debug.LogError("Enemy Detected");
                GetComponentInParent<AutoAttack>().AttackCheck(other.GetComponent<Watcher>());

            }
        }
    }
    
}

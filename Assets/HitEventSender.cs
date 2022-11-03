using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEventSender : MonoBehaviour
{
    EnemyScript enemyScript;
    private void Awake()
    {
        enemyScript = GetComponent<EnemyScript>();
    }
    public void HitEvent()
    {
        if (enemyScript)
            enemyScript.HitEventViaAnim();
    }
}

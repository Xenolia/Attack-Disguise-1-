using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherDetector : MonoBehaviour
{
    [SerializeField] float detectDuration;
    [SerializeField] float detectDurationReturn;
    [SerializeField] LineOfSight line;
    Transform target;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (target == other.transform)
                return;

            target = other.transform;
            if(detectDuration>0)
            detectDuration = detectDuration - Time.deltaTime;
           else
            {
                line.AddTarget(other.transform);
                detectDuration = detectDurationReturn;

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            detectDuration = detectDurationReturn;
        }
    }
}

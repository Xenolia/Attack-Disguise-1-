using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherDetector : MonoBehaviour
{
    [SerializeField] float detectDuration;
    [SerializeField] float detectDurationReturn;
    [SerializeField] LineOfSight line;
    Transform target;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (target == other.transform)
                return;

            target = other.transform;
          
                 line.AddTarget(other.transform);
             
        }
        /*
        if (other.gameObject.CompareTag("Detector"))
        {
            if (target == other.transform.parent)
                return;

            target = other.transform.parent;
           
                line.AddTarget(other.transform.parent);
        }
        */
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player") )
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
        if (other.gameObject.CompareTag("Detector"))
        {
            if (target == other.transform.parent)
                return;

            target = other.transform.parent;
            if (detectDuration > 0)
                detectDuration = detectDuration - Time.deltaTime;
            else
            {
                line.AddTarget(other.transform.parent);
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
        if (other.GetComponent<DetectorPlayer>() != null)
        {
            detectDuration = detectDurationReturn;
        }
    }
    */
}

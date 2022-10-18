using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
   [SerializeField] Transform targetTransform;
    [SerializeField] Vector3 offsetY;
 
    bool workOnce = true;
   
    public float multiplier;
    float screenHeight;
    private void Awake()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
       
    }
    // Update is called once per frame
    void Update()
    {
        multiplier = Screen.height / Screen.width;

        Vector3 targetPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        targetPosition = targetPosition + offsetY * multiplier;
        transform.position = targetPosition;
    }
}

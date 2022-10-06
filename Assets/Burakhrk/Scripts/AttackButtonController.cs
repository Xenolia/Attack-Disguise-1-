using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonController : MonoBehaviour
{
    Transform targetTransform;
    EnemyDetection enemyDetection;
    [SerializeField] Vector3 offsetY;
    public void Init(EnemyScript enemy,EnemyDetection enemyDetection)
    {
        this.enemyDetection = enemyDetection;
        targetTransform = enemy.transform;
        GetComponentInChildren<Button>().onClick.AddListener(()=>enemyDetection.SetNewTarget(enemy));
        UpdateButtonPos(targetTransform);
    }
    private void Update()
    {
        UpdateButtonPos(targetTransform);
    }

    private void UpdateButtonPos(Transform enemyTransform)
    {

        Vector3 targetPosition = Camera.main.WorldToScreenPoint(enemyTransform.position);
        targetPosition = targetPosition + offsetY;
        transform.position = targetPosition;
    }
}

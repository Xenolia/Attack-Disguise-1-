using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    private MovementInput movementInput;
    [SerializeField] CombatScript combatScript;

 //   public LayerMask layerMask;

    [SerializeField] Vector3 inputDirection;
    [SerializeField] private EnemyScript currentTarget;
    BattleManager battleManager;
    public GameObject cam;
     private void Start()
    {
        movementInput = GetComponentInParent<MovementInput>();
         battleManager = GameManager.Instance.GetComponent<BattleManager>();
    }

    private void Update()
    {
        var camera = Camera.main;
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * movementInput.moveAxis.y + right * movementInput.moveAxis.x;
        inputDirection = inputDirection.normalized;

        /*
        RaycastHit info;

        if (Physics.SphereCast(transform.position, 3f, inputDirection, out info, 10,layerMask))
        {
            if(info.collider.transform.GetComponent<EnemyScript>().IsAttackable())
                currentTarget = info.collider.transform.GetComponent<EnemyScript>();
        }
        */
    }
 
    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

 
    public void SetNewTarget(EnemyScript target)
    {
         if (combatScript.isAttackingEnemy)
            return;

 
        currentTarget = target;
             combatScript.Attack2();

    }
    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if(CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
        */
    }
}

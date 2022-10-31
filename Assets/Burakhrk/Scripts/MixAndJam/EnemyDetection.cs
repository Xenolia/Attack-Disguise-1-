using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    private MovementInput movementInput;
    [SerializeField] CombatScript combatScript;


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
    }

    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }


    public void SetNewTarget(EnemyScript target)
    {
        if (combatScript.isAttackingEnemy||combatScript.isDead)
        {
            return;
        }

        currentTarget = target;
        combatScript.Attack2();

    }
    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }
 
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;

public class CombatScript : MonoBehaviour
{
    EventBus eventBus;
    public bool OnBattle=false;
    public int Health;
    private EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private MovementInput movementInput;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;

    [Header("Target")]
   [SerializeField] private EnemyScript lockedTarget;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown;

    [Header("States")]
    public bool isAttackingEnemy = false;
    public bool isCountering = false;
    public bool isDead=false;
    [Header("Public References")]
    [SerializeField] private Transform punchPosition;
    [SerializeField] private ParticleSystemScript punchParticle;
    [SerializeField] private Transform takeDamagePos;
    [SerializeField] private GameObject takeDamageParticle;
    [SerializeField] private GameObject lastHitCamera;
    [SerializeField] private Transform lastHitFocusObject;

    //Coroutines
    private Coroutine counterCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine damageCoroutine;

    [Space]

    //Events
    public UnityEvent<EnemyScript> OnTrajectory;
    public UnityEvent<EnemyScript> OnHit;
    public UnityEvent<EnemyScript> OnCounterAttack;

    int animationCount = 0;
    string[] attacks;

    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponentInChildren<EnemyDetection>();
        movementInput = GetComponent<MovementInput>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }
    private void OnEnable()
    {
        eventBus=GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventBus>();
        eventBus.OnBattle += OnBattleBehaviour;
    }
    private void OnDisable()
    {
        eventBus.OnBattle -= OnBattleBehaviour;

    }
    void OnBattleBehaviour()
    {
        OnBattle = true;
    }
    private void LateUpdate()
    {

       // lockedTarget = enemyDetection.CurrentTarget();
    }
    //This function gets called whenever the player inputs the punch action
    void AttackCheck()
    {
        if (isAttackingEnemy)
            return;

         //Check to see if the detection behavior has an enemy set
        if (enemyDetection.CurrentTarget() == null)
        {
            if (enemyManager.AliveEnemyCount() == 0)
            {
               // Attack(null, 0);
                return;
            }
        }
     
        //Extra check to see if the locked target was set
            lockedTarget = enemyDetection.CurrentTarget();

        //AttackTarget
        Attack(lockedTarget, TargetDistance(lockedTarget));
    }

    public void Attack(EnemyScript target, float distance)
    {
         //Types of attack animation
        attacks = new string[] { "AirKick", "AirKick2", "AirPunch", "AirKick3" };

        //Attack nothing in case target is null
        if (target == null)
        {
            target = enemyDetection.CurrentTarget();
            /*
            AttackType("GroundPunch", .2f, null, 0);
            return;
            */
        }

        if (distance < 3)
        {
            animationCount = (int)Mathf.Repeat((float)animationCount + 1, (float)attacks.Length);
            string attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            AttackType(attackString, attackCooldown, target, .65f);
            Debug.Log("Close attack");
        }
        else
        {
            AttackType("AirKick", .5f, target, 0.75f);
            Debug.Log("Range attack");
        }

        //Change impulse
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);
    }
    void AttackStart()
    {
        movementInput.acceleration = 0;
        isAttackingEnemy = true;
        movementInput.enabled = false;
        lockedTarget.GettingAttacked();
        GameManager.Instance.EnableCanvas();
     }
    void AttackEnd()
    {
         movementInput.enabled = true;
        GameManager.Instance.DisableCanvas();
        enemyManager.StartAI();
    }

    void AttackType(string attackTrigger, float cooldown, EnemyScript target, float movementDuration)
    {
        animator.SetTrigger(attackTrigger);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(isLastHit() ? 1.5f : cooldown));

        //Check if last enemy
        if (isLastHit())
            StartCoroutine(FinalBlowCoroutine());

        if (target == null)
            return;

        target.StopMoving();
        MoveTorwardsTarget(target, movementDuration);
         IEnumerator AttackCoroutine(float duration)
        {
            AttackStart();
           
            yield return new WaitForSeconds(duration);
            isAttackingEnemy = false;           
            LerpCharacterAcceleration();
        }

        IEnumerator FinalBlowCoroutine()
        {
            Time.timeScale = .5f;
            lastHitCamera.SetActive(true);
            lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2);
            lastHitCamera.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log(" attack done last");

        }


    }

    void MoveTorwardsTarget(EnemyScript target, float duration)
    {
         OnTrajectory.Invoke(target) ;
        Vector3 targetPos=target.transform.position;
        transform.DOLookAt(targetPos, .2f);
        transform.DOMove(TargetOffset(target.transform), duration);
       
    }

    void CounterCheck()
    {
        return;
        //Initial check
        if (isCountering || isAttackingEnemy || !enemyManager.AnEnemyIsPreparingAttack())
            return;
        if (enemyDetection.CurrentTarget() == null)
 
        lockedTarget = enemyDetection.CurrentTarget();
        OnCounterAttack.Invoke(lockedTarget);

        if (TargetDistance(lockedTarget) > 2)
        {
            Attack(lockedTarget, TargetDistance(lockedTarget));
            return;
        }

        float duration = .3f;
        animator.SetTrigger("Dodge");
        transform.DOLookAt(lockedTarget.transform.position, .2f);
        transform.DOMove(transform.position + lockedTarget.transform.forward, duration);

        if (counterCoroutine != null)
            StopCoroutine(counterCoroutine);
        counterCoroutine = StartCoroutine(CounterCoroutine(duration));

        IEnumerator CounterCoroutine(float duration)
        {
            isCountering = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);

            Attack(lockedTarget, TargetDistance(lockedTarget));

            isCountering = false;

        }
    }

    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .95f);
    }

    public void HitEvent()
    {
       
        if (lockedTarget == null || enemyManager.AliveEnemyCount() == 0)
            return;
        OnHit.Invoke(lockedTarget);
        enemyDetection.CurrentTarget().OnPlayerHitBurak();
        //Polish
        punchParticle.PlayParticleAtPosition(punchPosition.position);
        Debug.LogError("hİT PARTİCLE");
        AttackEnd();

    }

    public void DamageEvent()
    {
        animator.SetTrigger("Hit");

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageCoroutine());
         
        IEnumerator DamageCoroutine()
        {
            movementInput.enabled = false;

            HealthCheck();

            yield return new WaitForSeconds(.2f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }
     }
    void HealthCheck()
    {

        Health--;
        GameManager.Instance.DisableCanvas();

        Debug.LogError("take daamage particle");

        takeDamageParticle.transform.position = takeDamagePos.transform.position;
        takeDamageParticle.GetComponent<ParticleSystem>().Play();

        if (Health<=0)
        {
            Dead();
        }

    }
    void Dead()
    {
        animator.SetTrigger("Death");
        eventBus.gameObject.GetComponent<GameManager>().LevelFail();
    }

    EnemyScript ClosestCounterEnemy()
    {
        float minDistance = 100;
        int finalIndex = 0;

        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            EnemyScript enemy = enemyManager.allEnemies[i].enemyScript;

            if (enemy.IsPreparingAttack())
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    finalIndex = i;
                }
            }
        }

        return enemyManager.allEnemies[finalIndex].enemyScript;

    }

    void LerpCharacterAcceleration()
    {
        movementInput.acceleration = 0;
        DOVirtual.Float(0, movementInput.acceleration, 0.4f, ((acceleration)=> movementInput.acceleration = acceleration));
    }

    bool isLastHit()
    {
        if (lockedTarget == null)
            return false;

        return enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1;
    }
    public void Attack2()
    {
        // Debug.Log("Attack triggered");
        AttackCheck();
    }
    #region Input

    private void OnCounter()
    {
        CounterCheck();
    }
    public void Attack()
    {
        return;
        // Debug.Log("Attack triggered");
        AttackCheck();
    }
    public void OnAttack()
    {
        if (!OnBattle)
            return;

        if (isAttackingEnemy)
            return;

      
    }

    #endregion

}

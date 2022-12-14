using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;

 
public class CombatScript : MonoBehaviour
{
    float isAttackingCounter=3;
     float isAttackingCounterReturner=3;

    Health health;
    BattleManager battleManager;
    public bool OnBattle=false;
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
    string attackString;
    [Space]

    //Events
    public UnityEvent<EnemyScript> OnTrajectory;
    public UnityEvent<EnemyScript> OnHit;
    public UnityEvent<EnemyScript> OnCounterAttack;

    int animationCount = 0;
    string[] attacks;
    private void Awake()
    {
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        health = GetComponent<Health>();
        enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponentInChildren<EnemyDetection>();
        movementInput = GetComponent<MovementInput>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        battleManager.OnBattle += OnBattleBehaviour;
        battleManager.OnBattleFinished += OnBattleFinished;

    }
    private void OnDisable()
    {
        battleManager.OnBattle -= OnBattleBehaviour;
        battleManager.OnBattleFinished -= OnBattleFinished;

    }
    void OnBattleBehaviour()
    {
        OnBattle = true;
    }
    void OnBattleFinished()
    {
        OnBattle = false;

    }
    private void Update()
    {
        if (!OnBattle)
            return;

        if(!isAttackingEnemy)
        {
            isAttackingCounter = isAttackingCounterReturner;
        }
        if(isAttackingEnemy&&isAttackingCounter>0)
        {
            isAttackingCounter = isAttackingCounter - Time.deltaTime;
        }
        if(isAttackingEnemy&&isAttackingCounter<=0)
        {
            isAttackingEnemy = false;
            isAttackingCounter = isAttackingCounterReturner;
        }
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
             attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            AttackType(attackString, attackCooldown, target, .65f);
            Debug.Log("Close attack");
        }
        else
        {
            AttackType("AirKick", .5f, target, 0.9f);
            Debug.Log("Range attack");
        }

        //Change impulse
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);
    }
    void AttackStart()
    {
        movementInput.allowMovement = false;
        movementInput.acceleration = 0;
        isAttackingEnemy = true;
        movementInput.enabled = false;
        lockedTarget.GettingAttacked();
        GameManager.Instance.DisableControl();
        animator.speed = 1.25f;
        
     }
    IEnumerator AttackEndNumerator()
    {
        if (attackString == "AirPunch")
        {
            yield return new WaitForSeconds(1f);
            movementInput.allowMovement = true;

            movementInput.enabled = true;
            isAttackingEnemy = false;
            yield return new WaitForSeconds(0.2f);
        }

        if (attackString == "AirKick")
        {
            yield return new WaitForSeconds(0.7f);
            movementInput.allowMovement = true;

            movementInput.enabled = true;
            isAttackingEnemy = false;
            yield return new WaitForSeconds(0.2f);
        }

        if (attackString == "AirKick2")
        {
            yield return new WaitForSeconds(0.5f);
            movementInput.allowMovement = true;

            movementInput.enabled = true;
            isAttackingEnemy = false;
            yield return new WaitForSeconds(0.2f);
        }
        if (attackString == "AirKick3")
        {
            yield return new WaitForSeconds(0.4f);
            movementInput.allowMovement = true;

            movementInput.enabled = true;
            isAttackingEnemy = false;
            yield return new WaitForSeconds(0.2f);
        }
        movementInput.allowMovement = true;

        enemyManager.StartAI();
        GameManager.Instance.EnableControl();
        animator.speed = 1;
      //  transform.DORotate(new Vector3(0, transform.rotation.y, transform.rotation.z), 0.3f);
    }
    void AttackEnd()
    {
        StartCoroutine(AttackEndNumerator());
    }

    void AttackType(string attackTrigger, float cooldown, EnemyScript target, float movementDuration)
    {
        AttackStart();

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
            
            yield return new WaitForSeconds(duration);
             AttackEnd();
            LerpCharacterAcceleration();
        }

        IEnumerator FinalBlowCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            Time.timeScale = .5f;
            lastHitCamera.SetActive(true);
            lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2);
            lastHitCamera.SetActive(false);
            Time.timeScale = 1f;
 
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
        /*
        if(!isLastHit())
         AttackEnd();
        */
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

            yield return new WaitForSeconds(.4f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }
     }
    void HealthCheck()
    {
        health.TakeDamage(1);

        movementInput.enabled = true;
         isAttackingEnemy = false;
        GameManager.Instance.EnableControl();
        // GameManager.Instance.EnableControl();


        takeDamageParticle.transform.position = takeDamagePos.transform.position;
        takeDamageParticle.GetComponent<ParticleSystem>().Play();
    }
   public void Dead()
    {
        animator.SetTrigger("Death");
        GameManager.Instance.DisableControl();
        StartCoroutine(WaitForDeadAnim());
    }
    IEnumerator WaitForDeadAnim()
    {
        yield return new WaitForSeconds(1.5f);
        battleManager.gameObject.GetComponent<GameManager>().LevelFail();

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
        DOVirtual.Float(0, movementInput.firstAcceleration, 1f, ((acceleration)=> movementInput.acceleration = acceleration));

    }

    bool isLastHit()
    {
        if (lockedTarget == null)
            return false;
        if(lockedTarget.GetComponent<EnemyScript>().watcherEnemy==true)
        {
            return false;
        }
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
        Debug.LogError("Counter check");
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

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;

public class EnemyScript : MonoBehaviour
{
    //mechanic blend burakhrk
    MovementInput movementInput;
    BattleManager battleManager;
    public bool OnBattle = false;
    public bool isTarget = false;
    public bool watcherEnemy = false;
    [SerializeField] float attackTimer = 2;
    [SerializeField] float attackTimerReturn = 2;

    [SerializeField] bool allowAttack = false;
    //Declarations
    private Animator animator;
    private CombatScript playerCombat;
    [SerializeField] EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private CharacterController characterController;

    [Header("Stats")]
    public bool isDead = false;
    public int health = 3;
    [SerializeField] float moveSpeed = 1;
    private Vector3 moveDirection;
    public float retreatDistance = 2;
    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    [Header("Polish")]
    [SerializeField] private ParticleSystem counterParticle;

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    private void Awake()
    {
        Watcher watcher = GetComponent<Watcher>();
        if (watcher)
            watcherEnemy = true;

        if(watcherEnemy)
        {
            attackTimerReturn = 2.2f;
            attackTimer = 0;
        }
        else
        {
            attackTimer = 1f;
            attackTimerReturn = 2.5f;
        }



         battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        characterController = GetComponent<CharacterController>();
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatScript>();
        animator = GetComponent<Animator>();
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
    void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();


        enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();

        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        MovementCoroutine = StartCoroutine(EnemyMovement());

    }

    IEnumerator EnemyMovement()
    {
        //Waits until the enemy is not assigned to no action like attacking or retreating
        yield return new WaitUntil(() => isWaiting == true);

        int randomChance = Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    void Update()
    {
        if (OnBattle && !isDead)
        {
            if (playerCombat.isDead)
                return;


            if (attackTimer > 0)
            {
                attackTimer = attackTimer - Time.deltaTime;
            }
            if (attackTimer <=0 ||isTarget)
            {
                if (playerCombat.isAttackingEnemy)
                {
                    if(attackTimer<1)
                    attackTimer = attackTimerReturn / (Random.Range(1, 3));

                    return;
                }
            }

                if (attackTimer <= 0 && !allowAttack)
            {
                allowAttack = true;
                attackTimer = attackTimerReturn;
            }
            //Constantly look at player
            transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

            //Only moves if the direction is set
            if (!isTarget && enemyManager)
            {
                MoveEnemy(moveDirection);
                return;
            }

            else if (!enemyManager)
            {
                MoveEnemyWatcher();
            }
        }
    }
    public void OnPlayerHitBurak()
    {

        StopEnemyCoroutines();
        DamageCoroutine = StartCoroutine(HitCoroutine());

        isLockedTarget = false;
        OnDamage.Invoke(this);

        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

        health--;

        if (health <= 0)
        {
            Death();
            return;
        }

        animator.SetTrigger("Hit");
        transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);

        StopMoving();

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            isMoving = false;
            yield return new WaitForSeconds(.5f);
            isStunned = false;

            if (!enemyManager)
                isMoving = true;
        }
        isTarget = false;
        SetRetreat();

        if (!enemyManager)
            isMoving = true;
    }
    //Listened event from Player Animation
    void OnPlayerHit(EnemyScript target)
    {
        return;
        if (target == this)
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            isLockedTarget = false;
            OnDamage.Invoke(this);

            health--;

            if (health <= 0)
            {
                Death();
                return;
            }
            if (Vector3.Distance(movementInput.transform.position, this.transform.position) > 1.7f)
                return;

            animator.SetTrigger("Hit");
            transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);

            StopMoving();
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(.5f);
            isStunned = false;
        }
    }

    void OnPlayerCounter(EnemyScript target)
    {
        if (target == this)
        {
            PrepareAttack(false);
        }
    }

    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            isTarget = false;
            StopMoving();
        }
    }
    public void GettingAttacked()
    {
        isTarget = true;
        StopMoving();
    }
    void Death()
    {
        if (isDead)
            return;

        isDead = true;
        StopEnemyCoroutines();
       
        Destroy(characterController);
         var testArray =  animator.GetCurrentAnimatorClipInfo(0);

            foreach(var test in testArray)
            {
            if (test.clip.empty)
                return;
             for (int i = 0; i < test.clip.events.Length; i++)
            {
                test.clip.events[i] = null;
            }

        }

animator.SetTrigger("Death");
 
         if (enemyManager)
        {
            enemyManager.UpdateEnemyList();
            enemyManager.SetEnemyAvailiability(this, false);
        }
        else
        {
            CombatController combatController = playerCombat.gameObject.GetComponent<CombatController>();
            combatController.ChangeMechanicToAuto();
            combatController.GetComponent<Health>().HideHealth();
        }
 
        var garbage = this;
        Destroy(garbage);
    }

    public void SetRetreat()
    {
        if (enemyDetection.CurrentTarget() == this)
            return;


        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > retreatDistance);
            isRetreating = false;
            StopMoving();

            //Free
            if (enemyManager)
            {
                isWaiting = true;

                MovementCoroutine = StartCoroutine(EnemyMovement());
            }
            else
            {
                isMoving = true;
            }

        }
    }

    public void SetAttack()
    {
        if (isPreparingAttack)
            return;
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }
    public void SetAttackAfterWatcher()
    {
        OnBattle = true;

        watcherEnemy = true;
        // StartCoroutine(SetAttackWatcherNumerator());
        MoveEnemyWatcher();
    }
    /*
    IEnumerator SetAttackWatcherNumerator()
    {
        if (isDead)
            yield break;
        yield return new WaitUntil(() => allowAttack == true);
 
                MoveEnemyWatcher();
                StartCoroutine(SetAttackWatcherNumerator());
     }
    */
    void PrepareAttack(bool active)
    {
        if (isDead)
            return;

        isPreparingAttack = active;

        if (active)
        {
            counterParticle.Play();
        }
        else
        {
            StopMoving();
            counterParticle.Clear();
            counterParticle.Stop();
        }
    }
    public float attackRange = 2;
    void MoveEnemy(Vector3 direction)
    {
        if (Vector3.Distance(transform.position, playerCombat.transform.position) >= attackRange)
        {
          //  Debug.Log("Move Enemy");
            //Set movespeed based on direction
            moveSpeed = 1;

            if (direction == Vector3.forward)
                moveSpeed = 5;
            if (direction == -Vector3.forward)
                moveSpeed = 2;

            //Set Animator values
            animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
            animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
            animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);


            Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
            Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
            Vector3 movedir = Vector3.zero;

            Vector3 finalDirection = Vector3.zero;

            if (direction == Vector3.forward)
                finalDirection = dir;
            if (direction == Vector3.right || direction == Vector3.left)
                finalDirection = (pDir * direction.normalized.x);
            if (direction == -Vector3.forward)
                finalDirection = -transform.forward;

            if (direction == Vector3.right || direction == Vector3.left)
                moveSpeed /= 1.5f;

            movedir += finalDirection * moveSpeed * Time.deltaTime;

            characterController.Move(movedir);
        }


        /*
             if (!isPreparingAttack)
         {
             if (Vector3.Distance(transform.position, playerCombat.transform.position) < attackRange)
             {
                 PrepareAttack(true);
             }
             return;

         }
        */
        float dist = Vector3.Distance(transform.position, playerCombat.transform.position);
        if (!allowAttack || attackTimer > 0)
        {
            if (dist < attackRange)
            {
                if(dist<=1.1f)
                animator.SetFloat("InputMagnitude", 0);
             }
            return;
        }
        if (dist < attackRange)
        {
            animator.SetFloat("InputMagnitude", 0);

           // Debug.Log("Attack called for " + transform.name);
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            {
                Attack();

            }
        }
    }
    bool setAttack = false;
    IEnumerator WaitForAllowAttack()
    {
        if (setAttack)
            yield break;
        setAttack = true;

        yield return new WaitUntil(() => allowAttack == true);
        setAttack = false;
        StopMoving();
        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            Attack();
        else
            PrepareAttack(false);
    }
    public float dirMultiplier = 0.6f;
    void MoveEnemyWatcher()
    {
        if (IsPreparingAttack() || isDead)
            return;


        float dist = Vector3.Distance(playerCombat.transform.position, transform.position);

        if (dist < attackRange)
        {
            isMoving = false;
            animator.SetFloat("InputMagnitude", 0);

            if (!setAttack)
                StartCoroutine(WaitForAllowAttack());
        }
        else
        {
            isMoving = true;
            characterController.Move(transform.forward * moveSpeed * 3 * Time.deltaTime);
            animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude));
          //  Debug.Log("getting in range ");

        }

    }
    private void Attack()
    {
        if (playerCombat.isDead || !allowAttack)
            return;
      //  Debug.Log("Attack called");
        allowAttack = false;

        float dist = Vector3.Distance(playerCombat.transform.position, transform.position);

        if (dist <= attackRange)
        {
            animator.SetTrigger("AirPunch");
        }
        PrepareAttack(false);

    }

    public void HitEventViaAnim()
    {
        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            playerCombat.DamageEvent();
        
        PrepareAttack(false);
    }

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        animator.SetFloat("InputMagnitude", 0);

    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
            {
                StopCoroutine(RetreatCoroutine);
                isRetreating = false;
            }
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    #region Public Booleans

    public bool IsAttackable()
    {
        return health > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion
}

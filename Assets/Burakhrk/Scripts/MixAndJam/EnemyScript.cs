using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    //mechanic blend burakhrk
    MovementInput movementInput;
    BattleManager battleManager;
    public bool OnBattle = false;
    public bool isTarget = false;
    public bool watcherEnemy = false;
    //Declarations
    private Animator animator;
    private CombatScript playerCombat;
    private EnemyManager enemyManager;
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
            //Constantly look at player
            transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

            //Only moves if the direction is set
            if (!isTarget && enemyManager)
                MoveEnemy(moveDirection);

            else if(characterController.velocity.magnitude==0)
            {
                isMoving = false;
                MoveEnemyWatcher();
            }
            else if(isMoving)
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
        }
        isTarget = false;
        SetRetreat();
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
        if (GetComponentInChildren<Canvas>())
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);


        Destroy(characterController);
        animator.SetTrigger("Death");

        if (enemyManager)
            enemyManager.SetEnemyAvailiability(this, false);
        else
        {
            CombatController combatController = playerCombat.gameObject.GetComponent<CombatController>();
            combatController.ChangeMechanicToAuto();
        }
        EnemyScript enemyScriptGarbage = this;
        Destroy(enemyScriptGarbage);
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
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
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
        StartCoroutine(SetAttackWatcherNumerator());
        MoveEnemyWatcher();
    }
    IEnumerator SetAttackWatcherNumerator()
    {
        if (isDead)
            yield break;

        yield return new WaitForSeconds(2);
        MoveEnemyWatcher();
        StartCoroutine(SetAttackWatcherNumerator());
    }

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
        Debug.Log("Move Enemy");
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

        //Don't do anything if isMoving is false
        if (!isMoving)
            return;

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
        Debug.Log(" Enemy moved checking is preparing attack ");

       
            if (!isPreparingAttack)
        {
            if (Vector3.Distance(transform.position, playerCombat.transform.position) < attackRange)
            {
                PrepareAttack(true);
            }
            return;

        }

        if (Vector3.Distance(transform.position, playerCombat.transform.position) < attackRange)
        {
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                Attack();
            else
                PrepareAttack(false);
        }
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

            //  StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            {
                PrepareAttack(true);
                Attack();
            }
        }
        else
        {
           
            PrepareAttack(false);
             characterController.Move(transform.forward*moveSpeed*3*Time.deltaTime);
            animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude));
            isMoving = true;
            Debug.Log("getting in range ");
           
        }

    }
    private void Attack()
    {
        if (playerCombat.isDead)
            return;

        isMoving = false;

        float dist = Vector3.Distance(playerCombat.transform.position, transform.position);

        if (dist <= attackRange)
        {
            animator.SetTrigger("AirPunch");
        }
        PrepareAttack(false); 
    }

    public void HitEvent()
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

        /*
        if (characterController)
            characterController.Move(moveDirection);
        */
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

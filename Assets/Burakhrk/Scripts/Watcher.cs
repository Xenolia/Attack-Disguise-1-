using MoreMountains.NiceVibrations;
using UnityEngine;
using Random = UnityEngine.Random;

public class Watcher : EnemyAI
{
    UIButtonController UIButton;
    private Animator animator;
    private Rigidbody rb;
    public float movementSpeed = 3f;
    GameObject player;

     public enum MovementDirection {RightLeft, UpDown };
   public MovementDirection movementDirection;
    [SerializeField] float range=2;
    [SerializeField] int sideRange = 1;

    public Material seenMaterial, unseenMaterial;
    public bool isDead = false;
    public bool isTarget = false;
    CharacterController characterController;

    private Vector3 firstPos;
    private Vector3 targetPos;
    bool hasButton = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        firstPos = transform.position;
        targetPos = firstPos;
        UIButton = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<UIButtonController>();
        player = GameObject.FindGameObjectWithTag("Player");
     }
    private void OnEnable()
    {
        lineOfSight.gameObject.SetActive(true);

    }
    private void OnDisable()
    {
        lineOfSight.gameObject.SetActive(false);
    }
    
    internal override void DoPatrol()
    {
        if (isDead||isTarget)
            return;
      
        if ((rb.position - targetPos).magnitude >= 0.3f)
        {
             MoveTo(targetPos);
        }
        else
        {
 

             while ((rb.position - targetPos).magnitude <= 0.3f)
            {
                animator.SetFloat("InputMagnitude", 0);

                if (movementDirection == MovementDirection.RightLeft)
                {
                    if (transform.position.magnitude - targetPos.magnitude > 0)
                        targetPos = (firstPos + new Vector3(Random.Range(1, range), 0, Random.Range(-sideRange, sideRange)));
                    else
                        targetPos = (firstPos + new Vector3(Random.Range(-range, -1), 0, Random.Range(-sideRange, sideRange)));

                }

                if (movementDirection == MovementDirection.UpDown)
                {
                    if (transform.position.magnitude - targetPos.magnitude > 0)
                        targetPos = (firstPos + new Vector3(Random.Range(-sideRange, sideRange), 0, Random.Range(1, range)));
                    else
                        targetPos = (firstPos + new Vector3(Random.Range(-sideRange, sideRange), 0, Random.Range(-range, -1)));

                }
            }

        }
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), out hit, 0.35F))
        {
  
 
            while ((rb.position - targetPos).magnitude <= 0.3f)
            {
                animator.SetFloat("InputMagnitude", 0);

                if (movementDirection == MovementDirection.RightLeft)
                {
                    if (transform.position.magnitude - targetPos.magnitude > 0)
                        targetPos = (firstPos + new Vector3(Random.Range(0, range), 0, Random.Range(-sideRange, sideRange)));
                    else
                        targetPos = (firstPos + new Vector3(Random.Range(-range, 0), 0, Random.Range(-sideRange, sideRange)));

                }

                if (movementDirection == MovementDirection.UpDown)
                {
                    if (transform.position.magnitude - targetPos.magnitude > 0)
                        targetPos = (firstPos + new Vector3(Random.Range(-sideRange, sideRange), 0, Random.Range(0, range)));
                    else
                        targetPos = (firstPos + new Vector3(Random.Range(-sideRange, sideRange), 0, Random.Range(-range, 0)));

                }
            }
        }
    }

    internal override void DoIdle()
    {
        if (isDead)
            return;

         SetAnim(false,false);
        return;
    }

    internal override void DoFollow()
    {
        if (isDead || isTarget)
            return;

        if (lineOfSight.visibleTargets.Count == 0)
            return;
        lineOfSight.SetMaterial(seenMaterial);
        lastKnownPos = lineOfSight.visibleTargets[0].position;
        lastKnownTime = Time.time;
        PlayerSeen();
    }

    internal override void DoAttack()
    {
        if (isDead || isTarget)
            return;

        SetAnim(false,false);
        PlayerSeen();

        Debug.Log("Pew Pew!");
    }
    void PlayerSeen()
    {
        if (!hasButton)
        {
            UIButton.CreateButton(GetComponent<EnemyScript>());
            hasButton = true;
            player.GetComponent<CombatController>().ChangeMechanicToButton();
            EnemyScript enemyScript = GetComponent<EnemyScript>();
            enemyScript.enabled = true;
          
            enemyScript.SetAttackAfterWatcher();
            player.GetComponent<Health>().ShowHealth();
            var sightObject = lineOfSight.gameObject;
            Destroy(sightObject);
            Watcher watcher = this;
            Destroy(watcher);
        }
    }
    void MoveTo(Vector3 pos)
    {
        if (isDead)
            return;
        characterController.enabled = false;
        SetAnim(true,false);
        Vector3 delta = (pos - transform.position).normalized;
          rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(delta),
           Time.fixedDeltaTime * movementSpeed*(1.5f)));
       rb.MovePosition(Vector3.Lerp(rb.position, rb.position + delta, Time.fixedDeltaTime * movementSpeed));
        characterController.enabled = true;

    }

    void SetAnim(bool isMoving, bool isDeadAnim)
    {
        if (isDeadAnim)
        {
            var testArray = animator.GetCurrentAnimatorClipInfo(0);

            foreach (var test in testArray)
            {
                 if(!test.clip.empty)
                test.clip.events = null;
            }
            animator.SetTrigger("Death");
            return;
        }
        if (isDead)
            return;

        if (isMoving)
       animator.SetFloat("InputMagnitude",0.55f);
       else
            animator.SetFloat("InputMagnitude", 0);
    }
    
    public void TakeDamage()
    {
        SetAnim(false, true);
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

        isDead = true;
         Debug.Log("watcher dead");
        DeadSituation();
    }
   public void DeadSituation()
    {
        UIButton.DestroyButton(GetComponent<EnemyScript>()) ;
          CharacterController character = GetComponent<CharacterController>();
        character.enabled = false;
         lineOfSight.gameObject.SetActive(false);
    }
}
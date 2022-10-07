using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Watcher : EnemyAI
{
    UIButtonController UIButton;
    private Animator animator;
    private Rigidbody rb;
    public float movementSpeed = 3f;
    public enum MovementDirection {RightLeft, UpDown };
   public MovementDirection movementDirection;
    [SerializeField] float range=2;
    public Material seenMaterial, unseenMaterial;
    public bool isDead = false;
    public bool isTarget = false;

    private Vector3 firstPos;
    private Vector3 targetPos;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        firstPos = transform.position;
        targetPos = firstPos;
        UIButton = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<UIButtonController>();
     }
    private Vector3 lastMove;

    internal override void DoPatrol()
    {
        if (isDead||isTarget)
            return;

        if ((rb.position - targetPos).magnitude >= 0.5f)
        {
            MoveTo(targetPos);
 
            /*
            if ((rb.position - lastMove).magnitude < 0.03) // We hit a wall
            {
                lastKnownPos = transform.position - transform.forward; // turn back
                Debug.Log("Watcher turn back");
            }
            */
        }
        else
        {
            if(movementDirection==MovementDirection.RightLeft)
            targetPos=(firstPos + new Vector3(Random.Range(-range, range), 0, 0));

            if (movementDirection == MovementDirection.UpDown)
                targetPos = (firstPos + new Vector3(0, 0, Random.Range(-range, range)));
            // Reached last known position, trying random walk.
            lineOfSight.SetMaterial(unseenMaterial);
          //  lastKnownPos = firstPos + new Vector3(Random.Range(-range, range), 0, firstPos.z);
        }
       // lastMove = rb.position;
    }

    internal override void DoIdle()
    {
        if (isDead)
            return;

        lineOfSight.SetMaterial(unseenMaterial);
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
        return;
        MoveTo(lineOfSight.visibleTargets[0].position); // move towards target
        lastKnownPos = lineOfSight.visibleTargets[0].position;
        lastKnownTime = Time.time;
    }

    internal override void DoAttack()
    {
        if (isDead || isTarget)
            return;

        SetAnim(false,false);

        Debug.LogError("Pew Pew!");
    }
    bool hasButton=false;
    void PlayerSeen()
    {
        if (!hasButton)
        {
            UIButton.CreateButton(GetComponent<EnemyScript>());
            hasButton = true;
        }
    }
    void MoveTo(Vector3 pos)
    {
        if (isDead)
            return;

        SetAnim(true,false);
        Vector3 delta = (pos - transform.position).normalized;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(delta),
            Time.fixedDeltaTime * movementSpeed));
        rb.MovePosition(Vector3.Lerp(rb.position, rb.position + delta, Time.fixedDeltaTime * movementSpeed));
    }

    void SetAnim(bool isMoving, bool isDeadAnim)
    {
        if (isDeadAnim)
        {
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
        isDead = true;
         Debug.Log("watcher dead");
        DeadSituation();
    }
    void DeadSituation()
    {
        GetComponent<CapsuleCollider>().enabled = false;
         CharacterController character = GetComponent<CharacterController>();
        character.enabled = false;
         lineOfSight.gameObject.SetActive(false);
    }
}
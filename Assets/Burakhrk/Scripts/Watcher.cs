using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Watcher : EnemyAI
{
    private Animator animator;
    private Rigidbody rb;
    public float movementSpeed = 3f;
    public Material seenMaterial, unseenMaterial;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }
    private Vector3 lastMove;

    internal override void DoPatrol()
    {
        if ((rb.position - lastKnownPos).magnitude > 0.5)
        {
            MoveTo(lastKnownPos);
            if ((rb.position - lastMove).magnitude < 0.03) // We hit a wall
                lastKnownPos = transform.position - transform.forward; // turn back
        }
        else
        {
            // Reached last known position, trying random walk.
            lineOfSight.SetMaterial(unseenMaterial);
            lastKnownPos = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        }
        lastMove = rb.position;
    }

    internal override void DoIdle()
    {
        lineOfSight.SetMaterial(unseenMaterial);
        SetAnim(false);
        return;
    }

    internal override void DoFollow()
    {
        if (lineOfSight.visibleTargets.Count == 0)
            return;
        lineOfSight.SetMaterial(seenMaterial);
        MoveTo(lineOfSight.visibleTargets[0].position); // move towards target
        lastKnownPos = lineOfSight.visibleTargets[0].position;
        lastKnownTime = Time.time;
    }

    internal override void DoAttack()
    {
        Debug.Log("Pew Pew!");
    }

    void MoveTo(Vector3 pos)
    {
        SetAnim(true);
        Vector3 delta = (pos - transform.position).normalized;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(delta),
            Time.fixedDeltaTime * movementSpeed));
        rb.MovePosition(Vector3.Lerp(rb.position, rb.position + delta, Time.fixedDeltaTime * movementSpeed));
    }

    void SetAnim(bool isMoving)
    {
        if(isMoving)
       animator.SetFloat("InputMagnitude",0.55f);
       else
            animator.SetFloat("InputMagnitude", 0);
    }
    
    public void TakeDamage()
    {

    }
}
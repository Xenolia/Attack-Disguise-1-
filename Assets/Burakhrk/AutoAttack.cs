using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{


    [SerializeField] bool isAttacking;
    [SerializeField] bool readyToAttack;
    [SerializeField] Watcher target;
    private string[] attacks;
    [SerializeField] float attackCooldown;


    [SerializeField] private Transform punchPosition;
    [SerializeField] private ParticleSystemScript punchParticle;


    private Coroutine attackCoroutine;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void AttackCheck(Watcher watcher)
    {
        Debug.Log("Auto attack check");

        if (!readyToAttack)
            return;
        target = watcher;
        if (target)
            Attack();

    }
    public void Attack()
    {
        isAttacking = true;

        //Types of attack animation
        attacks = new string[] { "AirKick", "AirKick2", "AirPunch", "AirKick3" };

            string attackString = attacks[Random.Range(0, attacks.Length)];
            AttackType(attackString, attackCooldown, target, .65f);
            Debug.Log("Close attack");
    }

    void AttackType(string attackTrigger, float cooldown, Watcher target, float movementDuration)
    {
        animator.SetTrigger(attackTrigger);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(cooldown));

        if (target == null)
            return;

        target.DoIdle();
        MoveTorwardsTarget(target, movementDuration);

        IEnumerator AttackCoroutine(float duration)
        {
            
            
            yield return new WaitForSeconds(duration);
            yield return new WaitForSeconds(0.5f);
            isAttacking = false;
            Debug.Log("attack done");

        }
    }

    void MoveTorwardsTarget(Watcher target, float duration)
    {
        transform.DOLookAt(target.transform.position, .2f);
        transform.DOMove((target.transform.position), duration);
    }

    public void HitEvent()
    {

        if (target == null||target.isDead)
            return;

        target.TakeDamage();
        //Polish
        punchParticle.PlayParticleAtPosition(punchPosition.position);
    }
}


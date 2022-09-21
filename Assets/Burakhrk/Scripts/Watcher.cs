using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Watcher : MonoBehaviour
{
    public Transform[] waypoints;
    [SerializeField] private int _currentWaypointIndex = 0;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [SerializeField] private float _waitTime = 1f; // in seconds
    [SerializeField] private float _waitCounter = 0f;
 [SerializeField]   private bool _waiting = false;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }
    private void Update()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            SetAnim(false);
            if (_waitCounter < _waitTime)
                return;
            _waiting = false;
        }

        Transform wp = waypoints[_currentWaypointIndex];
        if (Vector3.Distance(transform.position, wp.position) <stoppingDistance)
        {
            transform.position = wp.position;
            _waitCounter = 0f;
            _waiting = true;

            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
            SetAnim(false);

        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                wp.position,
                _speed * Time.deltaTime);
            transform.LookAt(wp.position);
            SetAnim(true);
        }
    }
   public void StopMoving()
    {
        _waiting = true;
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
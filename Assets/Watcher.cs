using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Watcher : MonoBehaviour
{
     Rigidbody2D rb;
    Collider2D coll;

    [SerializeField] Vector2 movement;
    [SerializeField] float rotationAngle;
    [SerializeField] float normalSpeed;
    [SerializeField] float alarmSpeed;
    [SerializeField] float restTime;
    [SerializeField] float initialrestTime;
    [SerializeField] float moveTime;
    [SerializeField] float initialmoveTime;
    [SerializeField] float lookTime;
    [SerializeField] float initiallookTime;
    int currenPatrolChoice = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        restTime = initialrestTime;
        moveTime = initialmoveTime;
        lookTime = initiallookTime;
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (checkBusy())
        {
            DoTask();
            UpdateTask();
        }
        else
        {
            StarTask();
        }
    }

    /// <summary>
    /// Checks if the uncle is busy performing something
    /// </summary>
    /// <returns><c>true</c>, if the time isn't over, <c>false</c> otherwise.</returns>
    bool checkBusy()
    {
        switch (currenPatrolChoice)
        {
            case 0:
                if (restTime > 0)
                {
                    return true;
                }
                else if (restTime <= 0)
                {
                    return false;
                }
                break;

            case 1:
                if (moveTime > 0)
                {
                    return true;
                }
                else if (moveTime <= 0)
                {
                    return false;
                }
                break;

            case 2:
                //I want him to stay still but rotate his field of view
                if (lookTime > 0)
                {
                    return true;
                }
                else if (lookTime <= 0)
                {
                    return false;
                }
                break;

            default:
                // return false too, the system is going wrong, and put unlce to sleep by default (lets make it robust)
                Debug.Log("this is a error, task beyong range: " + currenPatrolChoice);
                currenPatrolChoice = 0;
                return false;

                break;
        }
        return false;//just to keep happy the compiler
    }
    /// <summary>
    /// perform the task assigned... or just stay there
    /// </summary>
    void DoTask()
    {

        switch (currenPatrolChoice)
        {
            case 0:
                break;// it does nothing, just staying there added to the future?

            case 1:
                Move();
                break;

            case 2:
                //chek if this bbehavior is what you need here
                LookAround();
                break;

            default:
                Debug.Log("this is a error, task beyong range: " + currenPatrolChoice);//this is worst than the previous, the system jumps the if!
                break;
        }
    }

    /// <summary>
    /// Updates the Time, of the task assigned ofc.
    /// </summary>
    void UpdateTask()
    {
        switch (currenPatrolChoice)
        {
            case 0:
                Debug.Log("The Uncle is resting!");
                restTime -= Time.deltaTime; // each frame it will reduce the rest time
                print(restTime);
                break;

            case 1:
                Debug.Log("The Uncle is moving!");
                moveTime -= Time.deltaTime;
                print(moveTime);
                break;

            case 2:
                //I want him to stay still but rotate his field of view
                Debug.Log("The Uncle is Searching!");
                lookTime -= Time.deltaTime;
                print(lookTime);
                break;

            default:
                //we handled before the error inside the if
                break;
        }
    }

    /// <summary>
    /// initialize variables to start task... on the next frame :P
    /// </summary>
    void StarTask()
    {
        int currenPatrolChoice = Random.Range(0, 3);//las is exclusive, so dont count...
        switch (currenPatrolChoice)
        {
            case 0:
                restTime = initialrestTime;
                break;

            case 1:
                moveTime = initialmoveTime;
                movement = new Vector2(x: Random.Range(-1, 2) * normalSpeed, y: Random.Range(-1, 2) * normalSpeed);
                break;

            case 2:
                //lets give this life
                lookTime = initiallookTime;
                rotationAngle = Random.Range(-1, 1);//choose direction and speed?
                break;

            default:
                Debug.Log("task beyong range: " + currenPatrolChoice + "restarting");//the error was previously detected so, restart the sleep time
                restTime = initialrestTime;
                break;
        }
    }

    void Move()
    {
        rb.velocity = movement;
    }
    //check if this is the "looking behaviour" that you wanted
    void LookAround()
    {
        rb.MoveRotation(rotationAngle);
    }
}



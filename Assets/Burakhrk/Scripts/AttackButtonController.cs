using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;
    [SerializeField] GameObject textObj;

    Transform targetTransform;
    EnemyDetection enemyDetection;
    [SerializeField] Vector3 offsetY;
    public EnemyScript enemyScript;

    bool workOnce = true;
    MovementInput movementInput;
    UIButtonController UIButton;


    public float multiplier;
    float screenHeight;
    private void Awake()
    {
        UIButton = GetComponentInParent<UIButtonController>();
      
    }
    public void Init(EnemyScript enemy, EnemyDetection enemyDetection)
    {
        movementInput = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementInput>();
        this.enemyDetection = enemyDetection;
        targetTransform = enemy.transform;
        enemyScript = enemy;
        GetComponent<Button>().onClick.AddListener(() => enemyDetection.SetNewTarget(enemy));
        GetComponent<Button>().onClick.AddListener(() => DisableMovement());

        UpdateButtonPos(targetTransform);
    }
    void DisableMovement()
    {
        GameManager.Instance.DisableControl();
    }
    private void OnEnable()
    {
       UIButton.RegisterButton(this);
    }
    private void OnDisable()
    {
        UIButton.UnRegisterButton(this);
    }
    private void Update()
    {
         if (enemyScript.isDead)
        {
            if (workOnce)
            {
                workOnce = false;
                DisableButton();
            }
            return;
        }

        if (enemyScript.isTarget)
        {
            DisableButtonForAWhile();
            return;
        }

        UpdateButtonPos(targetTransform);
    }

    private void UpdateButtonPos(Transform enemyTransform)
    {
       
        Vector3 targetPosition = Camera.main.WorldToScreenPoint(enemyTransform.position);
        targetPosition = targetPosition + offsetY*multiplier;
        transform.position = targetPosition;
        //   Debug.LogError(ScaleCalculator());
    }
    float ScaleCalculator()
    {
        float distance = Vector3.Distance(movementInput.transform.position, enemyScript.transform.position);
        return distance;
    }
    void DisableButton()
    {
        gameObject.SetActive(false);
    }
    public void DisableButtonForAWhile()
    {
        image.enabled = false;
        button.enabled = false;
        textObj.SetActive(false);
    }
    public void EnableButton()
    {
        if (Time.timeScale != 1)
            return;

        textObj.SetActive(true);
        image.enabled = true;
        button.enabled = true;
    }
}
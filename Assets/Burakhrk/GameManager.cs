using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool controlEnabled=true;
    EnemyManager enemyManager;
     public UIManager uIManager;
    UIJoystickController joystickController;
    [SerializeField] UIButtonController uIButton;
    LevelManager levelManager;
    float enableCounter;
    bool isControlEnabled;
      private void Awake()
    {
         if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        AudioListener.volume = 0;
        levelManager = GetComponent<LevelManager>();
        joystickController = GetComponentInChildren<UIJoystickController>();
#if UNITY_EDITOR
#else
{
  Debug.logger.logEnabled = false;
}
#endif
    }
    private void OnEnable()
    {
        Time.timeScale = 1;
    }
    private void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("Enemy").GetComponentInParent<EnemyManager>();
    }
    public void LevelWin()
    {
        Debug.Log("Win level");
        Time.timeScale = 0;
        DisableControl();
        uIManager.ActivateWinPanel();
    }
    public void LevelFail()
    {
        Time.timeScale = 0;
        DisableControl();

        uIManager.ActivateLosePanel();

    }
    public void EnableControl()
    {
        enableCounter = 0;
        isControlEnabled = true;
        Debug.Log("Enable Control Button");

        uIButton.EnableAllButtons();
        joystickController.EnableJoyStick();
        controlEnabled = true;
    }
    public void DisableControl()
    {
        isControlEnabled = false;

        Debug.Log("Disable Control Button");
        uIButton.DisableAllButtons();
        joystickController.DisableJoyStick();
        controlEnabled = false;

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            LevelFail();  
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            levelManager.NextLevel();
        }
        if(!isControlEnabled)
        {
            enableCounter = enableCounter + Time.deltaTime;
            if(enableCounter>=5)
            {
                ForceEnableControl();
            }
        }
    }
    void ForceEnableControl()
    {
        EnableControl();
    }
}


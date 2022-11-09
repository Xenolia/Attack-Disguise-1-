using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool controlEnabled=true;
      public UIManager uIManager;
    UIJoystickController joystickController;
    [SerializeField] UIButtonController uIButton;
    LevelManager levelManager;
    float enableCounter=0;
    bool isControlEnabled=true;
      private void Awake()
    {
        Application.targetFrameRate = 30;
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
  Debug.unityLogger.logEnabled = false;
}
#endif
    }
    private void OnEnable()
    {
        Time.timeScale = 1;
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
    IEnumerator EnableStickDelay()
    {
        yield return new WaitForSeconds(0.15f);
        joystickController.EnableJoyStick();
        controlEnabled = true;

    }
    public void EnableControl()
    {
        enableCounter = 0;
        isControlEnabled = true;
 
        uIButton.EnableAllButtons();
        StartCoroutine(EnableStickDelay());
    }
    public void DisableControl()
    {
        isControlEnabled = false;

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
        if (!isControlEnabled)
        {
            enableCounter = enableCounter + Time.deltaTime;
            if (enableCounter >= 4f)
            {
                ForceEnableControl();
            }
        }
        else
            enableCounter = 0;
            
    }
    void ForceEnableControl()
    {
        if(!controlEnabled)
        EnableControl();
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    EnemyManager enemyManager;
     public UIManager uIManager;
    [SerializeField] UIButtonController uIButton;
    LevelManager levelManager;
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
        Debug.LogError("Win level");
        Time.timeScale = 0;
        DisableCanvas();
        uIManager.ActivateWinPanel();
    }
    public void LevelFail()
    {
        Time.timeScale = 0;
        DisableCanvas();

        uIManager.ActivateLosePanel();

    }
    public void DisableCanvas()
    {
         uIButton.EnableAllButtons();
    }
    public void EnableCanvas()
    {
        uIButton.DisableAllButtons();
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
    }
}


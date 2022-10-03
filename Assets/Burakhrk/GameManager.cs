using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    EnemyManager enemyManager;
    [SerializeField] GameObject disablePanel;
    public UIManager uIManager;
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
        disablePanel.SetActive(false);
        enemyManager.EnableButtonsEnemy();

    }
    public void EnableCanvas()
    {
        disablePanel.SetActive(true);
        enemyManager.DisableButtonsEnemy();

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


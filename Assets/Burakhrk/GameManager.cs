using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject disablePanel;
    public UIManager uIManager;
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
    }
    public void EnableCanvas()
    {
        disablePanel.SetActive(true);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            LevelFail();  
        }
    }
}


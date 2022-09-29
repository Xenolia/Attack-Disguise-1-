using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject disablePanel;
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
    }
    public void LevelWin()
    {
        Debug.LogError("Win level");
        Time.timeScale = 0;
    }
    public void LevelFail()
    {
        Time.timeScale = 0;
        disablePanel.SetActive(true);
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


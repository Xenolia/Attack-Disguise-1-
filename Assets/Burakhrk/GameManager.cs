using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject disablePanel;
    private void Awake()
    {
        AudioListener.volume = 0;
    }
    public void LevelWin()
    {

    }
    public void LevelFail()
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


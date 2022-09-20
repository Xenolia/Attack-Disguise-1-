using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Awake()
    {
        AudioListener.volume = 0;
    }
    public void LevelWin()
    {

    }
    public void LevelFail()
    {

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            LevelFail();  
        }
    }
}


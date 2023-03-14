using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class LevelManager : MonoBehaviour
{
    public GameObject[] Levels;
    [SerializeField] int levelNo = 1;
    [SerializeField] bool instantiate = false;
     [SerializeField] Button rewardedButton;

     private void Awake()
    {

        if (PlayerPrefs.HasKey("Level"))
        {
            levelNo = PlayerPrefs.GetInt("Level", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level", 1);
            levelNo = 1;
        }
     //   Debug.Log(Levels.Length);
       

        if (instantiate)
         CreateLevel(levelNo);
        else
            LoadLevel(levelNo);

    }
    
  
   public void x()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1));
        StopAllCoroutines();

        SceneManager.LoadScene(1);
    }
 
 
    public void LoadLevel(int levelNo)
    {
        foreach (var item in Levels)
        {
            item.SetActive(false);
        }
        int openLevelIndex;
        openLevelIndex = ((levelNo % Levels.Length));
        if (openLevelIndex != 0)
            openLevelIndex--;
        else
            openLevelIndex = Levels.Length-1;

        Debug.Log("openlevel Index  " + openLevelIndex);
         Levels[openLevelIndex].SetActive(true);
        Debug.Log("level no  "+levelNo);
        GetComponentInChildren<UIManager>().SetLevelText(levelNo);
    }
    void CreateLevel(int levelNo)
    {
        int openLevelIndex;
        openLevelIndex = ((levelNo % Levels.Length));
        if (openLevelIndex != 0)
            openLevelIndex--;
        else
            openLevelIndex = Levels.Length-1;

        foreach (var item in Levels)
        {
            GameObject garbage = item;
            if (item != Levels[openLevelIndex])
                Destroy(garbage);
        }
        Levels[openLevelIndex].SetActive(true);
        Debug.Log("level no  " + levelNo);
        GetComponentInChildren<UIManager>().SetLevelText(levelNo);
    }
    public void NextLevel()
    {
        
            LoadScene();

    }

    void LoadScene()
    {
       // CrazyEvents.Instance.GameplayStart();

        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
     
        StopAllCoroutines();
        SceneManager.LoadScene(1);
    }
    public void RestartLevel()
    {
            LoadRestartLevel();
     }
    void LoadRestartLevel()
    {
       // CrazyEvents.Instance.GameplayStart();
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1));
        StopAllCoroutines();
       
        SceneManager.LoadScene(1);
    }
}

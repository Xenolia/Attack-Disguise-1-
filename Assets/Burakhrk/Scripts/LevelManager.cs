using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public GameObject[] Levels;
    [SerializeField] int levelNo = 1;

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
         LoadLevel(levelNo);
    }
    public void LoadLevel(int levelNo)
    {
        foreach (var item in Levels)
        {
            item.SetActive(false);
        }
        int openLevelIndex;
        openLevelIndex = ((levelNo%Levels.Length));
        if (openLevelIndex != 0)
            openLevelIndex--;
        else
            openLevelIndex = 4;

        Debug.Log("Levels length" + Levels.Length);
        Debug.Log("openlevel Index  " + openLevelIndex);
         Levels[openLevelIndex].SetActive(true);
        Debug.Log("level no  "+levelNo);
        GetComponentInChildren<UIManager>().SetLevelText(levelNo);
    }
    public void NextLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        SceneManager.LoadScene(0);
    }
    public void RestartLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1));
        SceneManager.LoadScene(0);
    }
}

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
    [SerializeField] AdManager adManager;
    [SerializeField] Button rewardedButton;

    bool isOpened=false;
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
    void CheckAndRegisterRewardedEvents()
    {
       if(adManager.RewardedAdManager.IsRewardedAddReady())
        {
            rewardedButton.interactable = true;
        }
        else
        {
            rewardedButton.interactable = false;
            StartCoroutine(RegisterRewardedEvent());
        }
        adManager.RewardedAdManager.RegisterOnAddFailedToLoad(OnRewardedFailedLoading);
    }
    void OnRewardedFailedLoading(object sender , AdFailedToLoadEventArgs failedToLoadEventArgs)
    {
        CheckAndRegisterRewardedEvents();
    }
    private IEnumerator RegisterRewardedEvent()
    {
        while ( !(adManager.RewardedAdManager.IsRewardedAddReady()))
        {
            yield return null;
        }

        rewardedButton.interactable = true;
       // adManager.RewardedAdManager.RegisterOnUserEarnedReward(IsUserEarnedReward);
    }
    void IsUserEarnedReward(object sender , Reward reward)
    {
        LoadScene();
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
        isOpened = false;
        if (!adManager.InterstitialAdManager.IsInterstitialAdReady())
        {
            adManager.InterstitialAdManager.RequestInterstitial();
        }
        else
            isOpened = true;

        adManager.InterstitialAdManager.RegisterInterstitalAdOpening(SetIsOpenedTrue);
        adManager.InterstitialAdManager.RegisterInterstitialAdClosed(IntersitialAdClosed);

        adManager.InterstitialAdManager.ShowInterstitialAd();

        if (!isOpened)
            LoadScene();

    }
    void SetIsOpenedTrue(object sender, EventArgs ea)
    {
        isOpened = true;
    }
    void IntersitialAdClosed(object sender , EventArgs eventArgs)
    {
        LoadScene();
    }
    void IntersitialClosedForRestart(object sender, EventArgs eventArgs)
    {
        LoadRestartLevel();
    }

    public void RewardedButton()
    {
        adManager.RewardedAdManager.RegisterOnUserEarnedReward(IsUserEarnedReward);
        adManager.RewardedAdManager.ShowRewardedAd();
    }


    void LoadScene()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        SceneManager.LoadScene(1);
    }
    public void RestartLevel()
    {
        isOpened = false;
        if (!adManager.InterstitialAdManager.IsInterstitialAdReady())
        {
            adManager.InterstitialAdManager.RequestInterstitial();
        }
        else
            isOpened = true;

        adManager.InterstitialAdManager.RegisterInterstitalAdOpening(SetIsOpenedTrue);
        adManager.InterstitialAdManager.RegisterInterstitialAdClosed(IntersitialClosedForRestart);

        adManager.InterstitialAdManager.ShowInterstitialAd();

        if (!isOpened)
            LoadRestartLevel();

       
    }
    void LoadRestartLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1));
        SceneManager.LoadScene(1);
    }
}

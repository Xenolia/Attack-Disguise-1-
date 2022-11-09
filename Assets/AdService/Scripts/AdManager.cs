using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    private string _testBannerAdCode = "ca-app-pub-3940256099942544/6300978111";
    private string _testInterstitialAdCode = "ca-app-pub-3940256099942544/1033173712";
    private string _testRewardedAdCode = "ca-app-pub-3940256099942544/5224354917";
    private string _testRewardedInterstitialCode = "ca-app-pub-3940256099942544/5354046379";

    [Header("Ios Codes")]
    [SerializeField] private string _actualBannerAdCodeIos = "ca-app-pub-2311935022653953/2947744965";
    [SerializeField] private string _actualInterstitialAdCodeIos = "ca-app-pub-2311935022653953/4567114244";
    [SerializeField] private string _actualRewardedAdCodeIos;
    // [SerializeField] private string _actualRewardedInterstitialCodeIos;

    [Header("Android Codes")]
    [SerializeField] private string _actualBannerAdCodeAndroid;
    [SerializeField] private string _actualInterstatialCodeAndroid;
    [SerializeField] private string _actualRewardedAddCodeAndroid;

    [Header("Other Dependencies")]
    [SerializeField] private AdPosition _bannerAdPosition = AdPosition.Bottom;
    [SerializeField] private bool _isBuildVersion = false;

    private BannerAdManager _bannerAdManager;
    private InterstitialAdManager _interstitialAdManager;
    private RewardedAdManager _rewardedAdManager;
    public BannerAdManager BannerAdManager => _bannerAdManager;
    public InterstitialAdManager InterstitialAdManager => _interstitialAdManager;
    public RewardedAdManager RewardedAdManager => _rewardedAdManager;
    private void Awake()
    {
        Init();
    }
    private void OnDisable()
    {
        if (_bannerAdManager == null || _interstitialAdManager == null || _rewardedAdManager == null) return;
        _bannerAdManager.TerminateCurrentAd();
        _interstitialAdManager.TerminateCurrentAd();
        _rewardedAdManager.TerminateCurrentAdd();

        Debug.Log("Ads are terminated");
    }
    public void Init()
    {
        MobileAds.Initialize(initStatus => { });

        var bannerAdInitParamaters = new BannerAdManager.InitParamaters();
        bannerAdInitParamaters.AdPosition = _bannerAdPosition;

        var interstitialAdInitParamaters = new InterstitialAdManager.InitParameters();
        var rewardedAdInitParamaters = new RewardedAdManager.InitParameters();

        if (_isBuildVersion == false )
        {
            bannerAdInitParamaters.AdId = _testBannerAdCode;
            interstitialAdInitParamaters.AdId = _testInterstitialAdCode;
            rewardedAdInitParamaters.AdId = _testRewardedAdCode;
        }
        else
        {
           // Debug.Log("Test");
#if UNITY_IOS
            bannerAdInitParamaters.AdId = _actualBannerAdCodeIos;
            interstitialAdInitParamaters.AdId = _actualInterstitialAdCodeIos;
            rewardedAdInitParamaters.AdId = _actualRewardedAdCodeIos;
#elif UNITY_ANDROID
            bannerAdInitParamaters.AdId = _actualBannerAdCodeAndroid;
            interstitialAdInitParamaters.AdId = _actualInterstatialCodeAndroid;
            rewardedAdInitParamaters.AdId = _actualBannerAdCodeAndroid;
#else
            bannerAdInitParamaters.AdId = "unexpected_platform";
            interstitialAdInitParamaters.AdId = "unexpected_platform";
            rewardedAdInitParamaters.AdId = "unexpected_platform";
#endif
        }

        _bannerAdManager = new BannerAdManager(bannerAdInitParamaters);
        _interstitialAdManager =  new InterstitialAdManager(interstitialAdInitParamaters);
        _rewardedAdManager = new RewardedAdManager(rewardedAdInitParamaters);
    }
}
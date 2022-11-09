using GoogleMobileAds.Api;
using System;
using System.Diagnostics;
using UnityEngine;

public class InterstitialAdManager
{
    private InterstitialAd _interstitialAd;
	private string _adId;
    public InterstitialAdManager(InitParameters initParameters)
	{
		_adId = initParameters.AdId;
        RequestInterstitial();
	}

	public void RequestInterstitial()
	{
        if (_interstitialAd != null) TerminateCurrentAd();
        _interstitialAd = new InterstitialAd(_adId);
        AdRequest request = new AdRequest.Builder().Build();
        _interstitialAd.LoadAd(request);
        RegisterOnAddFailedToLoad(OnAddFailedToLoad);
        UnityEngine.Debug.Log("Interstitial request fro, Interstital Ad Manager");

    }

    public void RegisterOnAddFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnRewardedAddFailedToLoad)
    {
        _interstitialAd.OnAdFailedToLoad += OnRewardedAddFailedToLoad;
    }

    private void OnAddFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        UnityEngine.Debug.Log("Interstatial add couldnt load, requesting again");
        RequestInterstitial();
    }
    public class InitParameters
	{
		private string _adId;
		public string AdId { get => _adId; set => _adId = value; }
	}

    public void ShowInterstitialAd()
    {
        if (_interstitialAd.IsLoaded())
        {
            _interstitialAd.Show();
        }
    }

    public bool IsInterstitialAdReady()
    {
        return _interstitialAd.IsLoaded();
    }

    public void TerminateCurrentAd()
    {
        _interstitialAd.Destroy();
       
    }

    public void RegisterInterstitialAdReady(EventHandler<EventArgs> OnLoaded)
    {
        _interstitialAd.OnAdLoaded += OnLoaded;
    }

    public void RegisterInterstitialAdClosed(EventHandler<EventArgs> OnClosed)
    {
        _interstitialAd.OnAdClosed += OnClosed;
   }

    public void RegisterInterstitalAdOpening(EventHandler<EventArgs> OnOpening)
    {
        _interstitialAd.OnAdOpening += OnOpening;
    }

    public void RegisterInterstatialAdFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnFailedToLoad)
    {
        _interstitialAd.OnAdFailedToLoad += OnFailedToLoad;
    }

    public void RegisterInterstatialAdFailedToShow(EventHandler<AdErrorEventArgs> OnInterstatialAdFailedToShow)
    {
        _interstitialAd.OnAdFailedToShow += OnInterstatialAdFailedToShow;
    }

    private void InterstitialAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestInterstitial();
    }

    ////

    public void UnRegisterInterstitialAdReady(EventHandler<EventArgs> OnLoaded)
    {
        _interstitialAd.OnAdLoaded -= OnLoaded;
    }

    public void UnRegisterInterstitialAdClosed(EventHandler<EventArgs> OnClosed)
    {
        _interstitialAd.OnAdClosed -= OnClosed;
    }

    public void UnRegisterInterstitalAdOpening(EventHandler<EventArgs> OnOpening)
    {
        _interstitialAd.OnAdOpening -= OnOpening;
    }

    public void UnRegisterInterstatialAdFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnFailedToLoad)
    {
        _interstitialAd.OnAdFailedToLoad -= OnFailedToLoad;
    }

    public void UnRegisterInterstatialAdFailedToShow(EventHandler<AdErrorEventArgs> OnInterstatialAdFailedToShow)
    {
        _interstitialAd.OnAdFailedToShow -= OnInterstatialAdFailedToShow;
    }

    private void UnInterstitialAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestInterstitial();
    }
}
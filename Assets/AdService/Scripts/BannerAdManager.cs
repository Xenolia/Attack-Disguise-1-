using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAdManager
{
    private string _adId;
    private BannerView _bannerView;
    private AdPosition _adPosition;

    public BannerAdManager(InitParamaters initParamaters)
	{
        _adId = initParamaters.AdId;

        _adPosition = initParamaters.AdPosition;

        RequestBanner();
	}

    private void RequestBanner()
    {
        if (_bannerView != null) TerminateCurrentAd();
        _bannerView = new BannerView(_adId, AdSize.Banner, _adPosition);
        AdRequest request = new AdRequest.Builder().Build();
        _bannerView.LoadAd(request);
        RegisterOnAddFailedToLoad(OnAddFailedToLoad);
        Debug.Log("Banner request fro, Banner Ad Manager");
    }

    public void RegisterOnAddFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnRewardedAddFailedToLoad)
    {
        _bannerView.OnAdFailedToLoad += OnRewardedAddFailedToLoad;
    }

    private void OnAddFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestBanner();
        Debug.Log("Banner add couldnt load requesting again");
    }
    public class InitParamaters 
    {
        private string _adId;
        private AdPosition _adPosition;
        public string AdId { get => _adId; set => _adId = value; }
        public AdPosition AdPosition { get => _adPosition; set => _adPosition = value; }
    }

    public void TerminateCurrentAd()
    {
        _bannerView.Destroy();
    }
}
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedInterstitialAdManager
{
	private string _adId;
	private RewardedInterstitialAd _rewardedInterstitialAd;
	public RewardedInterstitialAdManager(InitParameters initParameters)
	{
		_adId = initParameters.AdId;
		RequestInterstitialRewarded();
	}

	private void RequestInterstitialRewarded()
	{
		//_rewardedInterstitialAd = new RewardedInterstitialAd();
	}

	public class InitParameters
	{
		private string _adId;
		public string AdId { get => _adId; set => _adId = value; }
	}
}

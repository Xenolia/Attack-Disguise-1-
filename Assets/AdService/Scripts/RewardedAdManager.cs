using GoogleMobileAds.Api;
using System;
using System.Diagnostics;

public class RewardedAdManager
{
	private RewardedAd _rewardedAd;

	private string _adId;
	public RewardedAdManager(InitParameters initParameters)
	{
		_adId = initParameters.AdId;

		RequestRewarded();
	}

	public void RequestRewarded()
	{
		if (_rewardedAd != null) TerminateCurrentAdd();
		_rewardedAd = new RewardedAd(_adId);
		AdRequest request = new AdRequest.Builder().Build();
		_rewardedAd.LoadAd(request);
		RegisterOnAddFailedToLoad(OnAddFailedToLoad);
        UnityEngine.Debug.Log("Rewarded request fro, Rewarded Ad Manager");

    }

    public class InitParameters
	{
		private string _adId;
		public string AdId { get => _adId; set => _adId = value; }
	}

	public void ShowRewardedAd()
	{
		if(_rewardedAd.IsLoaded())
		{
			_rewardedAd.Show();
		}
	}

	public bool IsRewardedAddReady()
	{
		return _rewardedAd.IsLoaded();
	}

	public void RegisterOnAddFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnRewardedAddFailedToLoad)
	{
		_rewardedAd.OnAdFailedToLoad += OnRewardedAddFailedToLoad;
    }

	private void OnAddFailedToLoad(object sender, AdFailedToLoadEventArgs e)
	{
		UnityEngine.Debug.Log("Rererded add manager : " + e.LoadAdError);
	}

    public void RegisterRewardedAdReady(EventHandler<EventArgs> OnRewardedAdReady)
	{
		_rewardedAd.OnAdLoaded += OnRewardedAdReady;
//		UnityEngine.Debug.Log("Rewarded add ready is fired");
	}

	public void TerminateCurrentAdd()
	{
		_rewardedAd.Destroy();
	}

	public void RegisterOnRewardedAdClosed(EventHandler<EventArgs> OnRewardedAdClosed)
	{
		_rewardedAd.OnAdClosed += OnRewardedAdClosed;
	}

	public void RegisterOnUserEarnedReward(EventHandler<Reward> OnUserEarnedReward)
	{
		_rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
	}

	public void RegisterOnRewardedAdFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnRewardedFailedToLoad)
	{
		_rewardedAd.OnAdFailedToLoad += OnRewardedFailedToLoad;
	}

	public void RegisterOnRewardedFailedToShow(EventHandler<AdErrorEventArgs> OnRewardedFailedToShow)
	{
		_rewardedAd.OnAdFailedToShow += OnRewardedFailedToShow;
	}

	public void RegisterOnRewardedAdOpening(EventHandler<EventArgs> OnRewardedAdOpening)
	{
		_rewardedAd.OnAdOpening += OnRewardedAdOpening;
	}

    /////

    public void UnRegisterRewardedAdReady(EventHandler<EventArgs> OnRewardedAdReady)
    {
        _rewardedAd.OnAdLoaded -= OnRewardedAdReady;
    }

    public void UnRegisterOnRewardedAdClosed(EventHandler<EventArgs> OnRewardedAdClosed)
    {
        _rewardedAd.OnAdClosed -= OnRewardedAdClosed;
    }

    public void UnRegisterOnUserEarnedReward(EventHandler<Reward> OnUserEarnedReward)
    {
        _rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
    }

    public void UnRegisterOnRewardedAdFailedToLoad(EventHandler<AdFailedToLoadEventArgs> OnRewardedFailedToLoad)
    {
        _rewardedAd.OnAdFailedToLoad -= OnRewardedFailedToLoad;
    }

    public void UnRegisterOnRewardedFailedToShow(EventHandler<AdErrorEventArgs> OnRewardedFailedToShow)
    {
        _rewardedAd.OnAdFailedToShow -= OnRewardedFailedToShow;
    }

    public void UnRegisterOnRewardedAdOpening(EventHandler<EventArgs> OnRewardedAdOpening)
    {
        _rewardedAd.OnAdOpening -= OnRewardedAdOpening;
    }
}
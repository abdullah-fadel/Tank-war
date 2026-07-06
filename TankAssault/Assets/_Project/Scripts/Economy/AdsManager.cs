using System;
using UnityEngine;

namespace TankAssault.Economy
{
    /// <summary>
    /// Abstraction over a rewarded-ads SDK (AdMob/Unity Ads/IronSource/etc). Swap
    /// MockAdsProvider for a real SDK adapter at integration time without touching game code.
    /// </summary>
    public interface IAdsProvider
    {
        bool IsRewardedAdReady();
        void ShowRewardedAd(Action onRewardGranted, Action onFailedOrCancelled);
    }

    public class MockAdsProvider : IAdsProvider
    {
        public bool IsRewardedAdReady() => true;

        public void ShowRewardedAd(Action onRewardGranted, Action onFailedOrCancelled)
        {
            Debug.Log("[MockAdsProvider] Simulating rewarded ad playback.");
            onRewardGranted?.Invoke();
        }
    }

    public class AdsManager : MonoBehaviour
    {
        private static IAdsProvider _provider = new MockAdsProvider();

        public static void SetProvider(IAdsProvider provider) => _provider = provider;

        public static bool IsRewardedAdReady() => _provider.IsRewardedAdReady();

        public static void ShowRewardedAdForCoins(int coinAmount)
        {
            _provider.ShowRewardedAd(
                onRewardGranted: () => CurrencyManager.Instance.AddCoins(coinAmount),
                onFailedOrCancelled: () => Debug.Log("[AdsManager] Rewarded ad failed or was cancelled."));
        }

        public static void ShowRewardedAdForGems(int gemAmount)
        {
            _provider.ShowRewardedAd(
                onRewardGranted: () => CurrencyManager.Instance.AddGems(gemAmount),
                onFailedOrCancelled: () => Debug.Log("[AdsManager] Rewarded ad failed or was cancelled."));
        }
    }
}

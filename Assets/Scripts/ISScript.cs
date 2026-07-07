using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ISScript : MonoBehaviour
{
    public static ISScript Instance { get; private set; }

#if UNITY_ANDROID
    string OrderIT = "2294896d5";
#elif UNITY_IOS
    string OrderIT = "";
#endif

    public string nextLevelName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        IronSource.Agent.init(OrderIT);
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        //Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;

        //Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;

        //RewardedAd Events
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
    }

    void OnApplicationPause(bool isPaused)
    {                 
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void SdkInitializationCompletedEvent()
    {
        IronSource.Agent.validateIntegration();
        LoadBanner();
        LoadFullAd(); // تحميل أول إعلان بيني فور جاهزية الـ SDK
        IronSource.Agent.launchTestSuite();
    }

    // --- Banner Management ---
    public void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner Loaded");
    void BannerOnAdLoadFailedEvent(IronSourceError error) => Debug.Log("Banner Load Failed");

    // --- Interstitial Management (إعادة تحميل ذكية) ---
    public void LoadFullAd()
    {
        if (!IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.loadInterstitial();
            Debug.Log("Loading Interstitial...");
        }
    }

    public void ShowFullAd()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("Interstitial not ready, loading one now.");
            LoadFullAd();
        }
    }

    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo) => Debug.Log("Interstitial Ready");

    void InterstitialOnAdLoadFailed(IronSourceError error) 
    {
        Debug.Log("Interstitial Load Failed. Retrying in 5 seconds...");
        Invoke(nameof(LoadFullAd), 5f); // إعادة المحاولة بعد 5 ثوانٍ لتجنب الضغط على الشبكة في حال عدم وجود إنترنت
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Closed. Prefetching next ad.");
        LoadFullAd(); // تحميل الإعلان القادم فور إغلاق الإعلان الحالي ليكون جاهزاً دائماً!
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Show Failed. Reloading...");
        LoadFullAd();
    }

    // --- Rewarded Video Management ---
    public void ShowRewardedAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Rewarded Video not available right now.");
        }
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Closed.");
        // IronSource يعيد تحميل الفيديو تلقائياً في معظم الأحيان، ولكن كخطوة أمان إضافية:
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("User Rewarded!");
        SceneManager.LoadScene(nextLevelName);
        Time.timeScale = 1f;
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Show Failed.");
    }
}
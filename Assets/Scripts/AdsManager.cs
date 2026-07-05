using UnityEngine;
using System;
// Official modern LevelPlay library
using Unity.Services.LevelPlay;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [Header("App Keys")]
    [SerializeField] private string androidAppKey = "YOUR_ANDROID_APP_KEY";
    [SerializeField] private string iosAppKey = "YOUR_IOS_APP_KEY";

    [Header("Test Mode")]
    [Tooltip("When enabled, official ironSource test App Keys will be used instead of your real ones. Make sure to disable this before releasing the game.")]
    [SerializeField] private bool useTestAds = false;

    // Official ironSource test App Keys - these always return test ads only, 100% safe with no policy violation risk
    private const string TEST_ANDROID_APP_KEY = "85460dcd";
    private const string TEST_IOS_APP_KEY = "8545d445";

    [Header("Ad Unit IDs")]
    [SerializeField] private string bannerAdUnitId = "YOUR_BANNER_ID";
    [SerializeField] private string interstitialAdUnitId = "YOUR_INTERSTITIAL_ID";
    [SerializeField] private string rewardedAdUnitId = "YOUR_REWARDED_ID";

    // Using the modern ad unit classes
    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedAd;

    public static event Action OnRewardedSuccess;

    // Fixed prefix for all logs from this script so it's easy to filter in the Console/Logcat
    private const string LOG_TAG = "[AdsManager]";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAds()
    {
        string currentAppKey = GetAppKey();

        Debug.Log($"{LOG_TAG} 🚀 Starting LevelPlay initialization... App Key in use: {currentAppKey} | Test Mode: {useTestAds}");

        if (string.IsNullOrEmpty(currentAppKey))
        {
            Debug.LogError($"{LOG_TAG} ❌ Fatal error: App Key is empty! Make sure you set a valid App Key in the Inspector for each platform (Android/iOS).");
            return;
        }

        if (useTestAds)
        {
            Debug.LogWarning($"{LOG_TAG} ⚠️ Test Ads mode is ENABLED! All ads shown will be test ads only. Don't forget to disable this before release.");
        }

        // Must subscribe to these events BEFORE calling Init
        LevelPlay.OnInitSuccess += OnInitializationComplete;
        LevelPlay.OnInitFailed += OnInitializationFailed;

        // The modern Init only takes: appKey, plus optional userId and adFormats
        LevelPlay.Init(currentAppKey);
    }

    private string GetAppKey()
    {
        if (useTestAds)
        {
            #if UNITY_ANDROID
            return TEST_ANDROID_APP_KEY;
            #elif UNITY_IOS
            return TEST_IOS_APP_KEY;
            #else
            return "";
            #endif
        }

        #if UNITY_ANDROID
        return androidAppKey;
        #elif UNITY_IOS
        return iosAppKey;
        #else
        return "";
        #endif
    }

    private void OnInitializationComplete(LevelPlayConfiguration config)
    {
        Debug.Log($"{LOG_TAG} ✅ LevelPlay initialized successfully! Creating and loading ads now...");
        SetupAds();
    }

    private void OnInitializationFailed(LevelPlayInitError error)
    {
        Debug.LogError($"{LOG_TAG} ❌ LevelPlay initialization FAILED! Reason: {error.ErrorMessage} | Error Code: {error.ErrorCode}\n" +
                        $"Possible causes: wrong App Key - no internet connection - the app doesn't exist in the dashboard.");
    }

    private void SetupAds()
    {
        // Prepare all ad types after successful initialization
        CreateBannerAd();
        CreateInterstitialAd();
        CreateRewardedAd();
    }

    #region Banner Ads
    private void CreateBannerAd()
    {
        if (string.IsNullOrEmpty(bannerAdUnitId) || bannerAdUnitId == "YOUR_BANNER_ID")
        {
            Debug.LogError($"{LOG_TAG} ❌ [Banner] Ad Unit Id is empty or still the default placeholder! Set the correct ID from the dashboard.");
            return;
        }

        Debug.Log($"{LOG_TAG} 🟦 [Banner] Creating banner with Ad Unit Id: {bannerAdUnitId}");

        // The installed SDK version uses a Config Builder instead of passing size/position directly to the constructor
        var bannerConfig = new LevelPlayBannerAd.Config.Builder()
            .SetSize(LevelPlayAdSize.BANNER)
            .SetPosition(LevelPlayBannerPosition.BottomCenter)
            .SetDisplayOnLoad(true)
            .Build();

        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, bannerConfig);

        // Subscribe to banner ad events - success and failure at every stage
        bannerAd.OnAdLoaded += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Banner] Banner loaded successfully! Showing it on screen now.");
            ShowBanner(); // Make sure the banner is actually shown on screen right after a successful load
        };

        bannerAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Banner] Banner FAILED to load! Reason: {error.ErrorMessage} | Error Code: {error.ErrorCode}\n" +
                            $"Possible causes: wrong Ad Unit Id - the Ad Unit is paused/inactive - no ad network is active on it - the app isn't Live yet.");
        };

        bannerAd.OnAdDisplayed += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Banner] Banner successfully displayed on screen (impression recorded).");
        };

        bannerAd.OnAdDisplayFailed += (info, error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Banner] Banner loaded but FAILED to display on screen! Reason: {error.ErrorMessage}");
        };

        bannerAd.OnAdClicked += (info) =>
        {
            Debug.Log($"{LOG_TAG} 🖱️ [Banner] User clicked the banner.");
        };

        bannerAd.LoadAd(); // Correct method name is LoadAd, not Load
        Debug.Log($"{LOG_TAG} ⏳ [Banner] Load request sent... waiting for response.");
    }

    public void ShowBanner()
    {
        if (bannerAd == null)
        {
            Debug.LogWarning($"{LOG_TAG} ⚠️ [Banner] Can't show the banner - it hasn't been created yet.");
            return;
        }
        bannerAd.ShowAd(); // ShowAd, not Show
    }

    public void HideBanner()
    {
        if (bannerAd == null)
        {
            Debug.LogWarning($"{LOG_TAG} ⚠️ [Banner] Can't hide the banner - it hasn't been created yet.");
            return;
        }
        bannerAd.HideAd(); // HideAd, not Hide
        Debug.Log($"{LOG_TAG} 🙈 [Banner] Banner hidden.");
    }
    #endregion

    #region Interstitial Ads
    private void CreateInterstitialAd()
    {
        if (string.IsNullOrEmpty(interstitialAdUnitId) || interstitialAdUnitId == "YOUR_INTERSTITIAL_ID")
        {
            Debug.LogError($"{LOG_TAG} ❌ [Interstitial] Ad Unit Id is empty or still the default placeholder! Set the correct ID from the dashboard.");
            return;
        }

        Debug.Log($"{LOG_TAG} 🟨 [Interstitial] Creating interstitial ad with Ad Unit Id: {interstitialAdUnitId}");

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Interstitial] Interstitial loaded successfully! Ready to show now.");
        };

        interstitialAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Interstitial] Interstitial FAILED to load! Reason: {error.ErrorMessage} | Error Code: {error.ErrorCode}\n" +
                            $"Possible causes: wrong Ad Unit Id - the Ad Unit is paused/inactive - no ad network is active on it - the app isn't Live yet.");
        };

        interstitialAd.OnAdDisplayed += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Interstitial] Interstitial successfully displayed on screen (impression recorded).");
        };

        interstitialAd.OnAdDisplayFailed += (info, error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Interstitial] Ad loaded but FAILED to display on screen! Reason: {error.ErrorMessage}");
        };

        interstitialAd.OnAdClicked += (info) =>
        {
            Debug.Log($"{LOG_TAG} 🖱️ [Interstitial] User clicked the ad.");
        };

        interstitialAd.OnAdClosed += (info) =>
        {
            Debug.Log($"{LOG_TAG} 🔒 [Interstitial] User closed the ad. Loading a new one automatically...");
            interstitialAd.LoadAd(); // Reload a new ad once the current one is closed
        };

        interstitialAd.LoadAd(); // LoadAd, not Load
        Debug.Log($"{LOG_TAG} ⏳ [Interstitial] Load request sent... waiting for response.");
    }

    public void ShowInterstitial()
    {
        if (interstitialAd == null)
        {
            Debug.LogError($"{LOG_TAG} ❌ [Interstitial] Can't show the ad - it hasn't been created yet. Make sure initialization (Init) succeeded.");
            return;
        }

        if (interstitialAd.IsAdReady())
        {
            Debug.Log($"{LOG_TAG} ▶️ [Interstitial] Ad is ready - showing it now.");
            interstitialAd.ShowAd(); // ShowAd, not Show
        }
        else
        {
            Debug.LogWarning($"{LOG_TAG} ⚠️ [Interstitial] Ad is not ready yet (still loading, or a previous load failed). Retrying...");
            interstitialAd.LoadAd();
        }
    }
    #endregion

    #region Rewarded Ads
    private void CreateRewardedAd()
    {
        if (string.IsNullOrEmpty(rewardedAdUnitId) || rewardedAdUnitId == "YOUR_REWARDED_ID")
        {
            Debug.LogError($"{LOG_TAG} ❌ [Rewarded] Ad Unit Id is empty or still the default placeholder! Set the correct ID from the dashboard.");
            return;
        }

        Debug.Log($"{LOG_TAG} 🟩 [Rewarded] Creating rewarded ad with Ad Unit Id: {rewardedAdUnitId}");

        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewardedAd.OnAdLoaded += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Rewarded] Rewarded ad loaded successfully! Ready to show now.");
        };

        rewardedAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Rewarded] Rewarded ad FAILED to load! Reason: {error.ErrorMessage} | Error Code: {error.ErrorCode}\n" +
                            $"Possible causes: wrong Ad Unit Id - the Ad Unit is paused/inactive - no ad network is active on it - the app isn't Live yet.");
        };

        rewardedAd.OnAdDisplayed += (info) =>
        {
            Debug.Log($"{LOG_TAG} ✅ [Rewarded] Rewarded ad successfully displayed on screen (impression recorded).");
        };

        rewardedAd.OnAdDisplayFailed += (info, error) =>
        {
            Debug.LogError($"{LOG_TAG} ❌ [Rewarded] Ad loaded but FAILED to display on screen! Reason: {error.ErrorMessage}");
        };

        rewardedAd.OnAdClicked += (info) =>
        {
            Debug.Log($"{LOG_TAG} 🖱️ [Rewarded] User clicked the ad.");
        };

        rewardedAd.OnAdRewarded += (info, reward) =>
        {
            Debug.Log($"{LOG_TAG} 🏆 [Rewarded] Success! The user watched the full ad and earned the reward.");
            OnRewardedSuccess?.Invoke();
        };

        rewardedAd.OnAdClosed += (info) =>
        {
            Debug.Log($"{LOG_TAG} 🔒 [Rewarded] User closed the ad. Loading a new one automatically...");
            rewardedAd.LoadAd();
        };

        rewardedAd.LoadAd(); // LoadAd, not Load
        Debug.Log($"{LOG_TAG} ⏳ [Rewarded] Load request sent... waiting for response.");
    }

    public void ShowRewarded()
    {
        if (rewardedAd == null)
        {
            Debug.LogError($"{LOG_TAG} ❌ [Rewarded] Can't show the ad - it hasn't been created yet. Make sure initialization (Init) succeeded.");
            return;
        }

        if (rewardedAd.IsAdReady())
        {
            Debug.Log($"{LOG_TAG} ▶️ [Rewarded] Ad is ready - showing it now.");
            rewardedAd.ShowAd(); // ShowAd, not Show
        }
        else
        {
            Debug.LogWarning($"{LOG_TAG} ⚠️ [Rewarded] Ad is not ready yet (still loading, or a previous load failed). Retrying...");
            rewardedAd.LoadAd();
        }
    }
    #endregion

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnInitializationComplete;
        LevelPlay.OnInitFailed -= OnInitializationFailed;
    }
}
//AdsManager.Instance.ShowInterstitial();
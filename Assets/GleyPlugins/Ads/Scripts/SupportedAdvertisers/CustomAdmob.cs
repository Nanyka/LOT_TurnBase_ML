namespace GleyMobileAds
{
    using UnityEngine.Events;
    using UnityEngine;
#if USE_ADMOB
    using GoogleMobileAds.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
#endif

    public class CustomAdmob : MonoBehaviour, ICustomAds
    {
#if USE_ADMOB
        private const float reloadTime = 30;

        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private InterstitialAd interstitial;
        private BannerView banner;
        private RewardedAd rewardedVideo;
        private BannerPosition position;
        private BannerType bannerType;
        private string rewardedVideoId;
        private string interstitialId;
        private string bannerId;
        private string consent;
        private string ccpaConsent;
        private string designedForFamilies;
        private readonly int maxRetryCount = 10;
        private int currentRetryRewardedVideo;
        private int currentRetryInterstitial;
        private bool directedForChildren;
        private bool debug;
        private bool initialized;
        private bool watched;
        private bool bannerLoaded;
        private bool bannerUsed;
        private bool interstitialFailedToLoad;
        private bool rewardedVideoFailedToLoad;
        private bool forceReload;

        #region Initialize
        /// <summary>
        /// Initializing Admob
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                if (debug)
                {
                    Debug.Log("Admob Start Initialization");
                    ScreenWriter.Write("Admob Start Initialization");
                }

                //get settings
#if UNITY_ANDROID
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif
                //apply settings
                interstitialId = settings.idInterstitial.id;
                bannerId = settings.idBanner.id;
                rewardedVideoId = settings.idRewarded.id;

                TagForChildDirectedTreatment tagFororChildren;
                TagForUnderAgeOfConsent tagForUnderAge;
                MaxAdContentRating contentRating;
                directedForChildren = settings.directedForChildren;

                if (settings.directedForChildren == true)
                {
                    designedForFamilies = "true";
                    tagFororChildren = TagForChildDirectedTreatment.True;
                    tagForUnderAge = TagForUnderAgeOfConsent.True;
                    contentRating = MaxAdContentRating.G;
                }
                else
                {
                    designedForFamilies = "false";
                    tagFororChildren = TagForChildDirectedTreatment.Unspecified;
                    tagForUnderAge = TagForUnderAgeOfConsent.Unspecified;
                    contentRating = MaxAdContentRating.Unspecified;
                }


                RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(tagFororChildren)
                .SetMaxAdContentRating(contentRating)
                .SetTagForUnderAgeOfConsent(tagForUnderAge).build();
                MobileAds.SetRequestConfiguration(requestConfiguration);

                MobileAds.SetiOSAppPauseOnBackground(true);

                //verify settings
                if (debug)
                {
                    Debug.Log("Admob Banner ID: " + bannerId);
                    ScreenWriter.Write("Admob Banner ID: " + bannerId);
                    Debug.Log("Admob Interstitial ID: " + interstitialId);
                    ScreenWriter.Write("Admob Interstitial ID: " + interstitialId);
                    Debug.Log("Admob Rewarded Video ID: " + rewardedVideoId);
                    ScreenWriter.Write("Admob Rewarded Video ID: " + rewardedVideoId);
                    Debug.Log("Admob Directed for children: " + directedForChildren);
                    ScreenWriter.Write("Admob Directed for children: " + directedForChildren);
                }

                //preparing Admob SDK for initialization
                if (consent == UserConsent.Unset || consent == UserConsent.Accept)
                {
                    this.consent = "0";
                }
                else
                {
                    this.consent = "1";
                }

                if (ccpaConsent == UserConsent.Unset || ccpaConsent == UserConsent.Accept)
                {
                    this.ccpaConsent = "0";
                }
                else
                {
                    this.ccpaConsent = "1";
                }

                MobileAds.RaiseAdEventsOnUnityMainThread = true;
                MobileAds.SetiOSAppPauseOnBackground(true);

                MobileAds.Initialize(InitComplete);

                initialized = true;
            }
        }


        private void InitComplete(InitializationStatus status)
        {
            if (debug)
            {
                Debug.Log(this + " Init Complete: ");
                ScreenWriter.Write(this + " Init Complete: ");
                Dictionary<string, AdapterStatus> adapterState = status.getAdapterStatusMap();
                foreach (var adapter in adapterState)
                {
                    ScreenWriter.Write(adapter.Key + " " + adapter.Value.InitializationState + " " + adapter.Value.Description);
                }
            }
            if (!string.IsNullOrEmpty(rewardedVideoId))
            {
                //start loading ads
                LoadRewardedVideo();
            }
            if (!string.IsNullOrEmpty(interstitialId))
            {
                LoadInterstitial();
            }
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if (consent == UserConsent.Unset || consent == UserConsent.Accept)
            {
                this.consent = "0";
            }
            else
            {
                this.consent = "1";
            }

            if (ccpaConsent == UserConsent.Unset || ccpaConsent == UserConsent.Accept)
            {
                this.ccpaConsent = "0";
            }
            else
            {
                this.ccpaConsent = "1";
            }
            if (debug)
            {
                Debug.Log("Admob Update consent to " + consent + " and CCPA " + ccpaConsent);
                ScreenWriter.Write("Admob Update consent to " + consent + " and CCPA " + ccpaConsent);
            }
        }
        #endregion


        AdRequest CreateRequest()
        {
            AdRequest.Builder request = new AdRequest.Builder();
            request.AddExtra("npa", consent);
            request.AddExtra("is_designed_for_families", designedForFamilies);
            request.AddExtra("rdp", ccpaConsent);
            return request.Build();
        }


        #region Interstitial
        /// <summary>
        /// Check if Admob interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            if (interstitial != null)
            {
                return interstitial.CanShowAd();
            }
            return false;
        }


        /// <summary>
        /// Show Admob interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial </param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                interstitial.Show();
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + "Interstitial ad cannot be shown.");
                    ScreenWriter.Write(this + "Interstitial ad cannot be shown.");
                }
            }
        }


        /// <summary>
        /// Show Admob interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                interstitial.Show();
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + "Interstitial ad cannot be shown.");
                    ScreenWriter.Write(this + "Interstitial ad cannot be shown.");
                }
            }
        }


        /// <summary>
        /// Loads an Admob interstitial
        /// </summary>
        private void LoadInterstitial()
        {
            if (debug)
            {
                Debug.Log("Admob Start Loading Interstitial");
                ScreenWriter.Write("Admob Start Loading Interstitial");
            }

            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.OnAdPaid -= InterstitialAdPaied;
                interstitial.OnAdImpressionRecorded -= ImpressionRecorded;
                interstitial.OnAdClicked -= AdClicked;
                interstitial.OnAdFullScreenContentOpened -= AdFullScreenOpened;
                interstitial.OnAdFullScreenContentClosed -= AdFullScreenClosed;
                interstitial.OnAdFullScreenContentFailed -= AdFullScreenFailed;
                interstitial.Destroy();
                interstitial = null;
            }

            // create our request used to load the ad.
            var adRequest = CreateRequest();

            // Load an interstitial ad
            InterstitialAd.Load(interstitialId, adRequest, InterstitialLoadCallback);
        }


        private void InterstitialLoadCallback(InterstitialAd ad, LoadAdError loadAdError)
        {
            // if error is not null, the load request failed.
            if (loadAdError != null || ad == null)
            {
                InterstitialFailed(loadAdError);
                return;
            }
            else
            {
                InterstitialLoaded(ad);
            }
            interstitial = ad;
        }


        /// <summary>
        /// Admob specific event triggered after an interstitial was loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterstitialLoaded(InterstitialAd ad)
        {
            interstitial = ad;
            if (debug)
            {
                Debug.Log("Admob Interstitial Loaded");
                ScreenWriter.Write("Admob Interstitial Loaded");
            }

            interstitial.OnAdPaid += InterstitialAdPaied;
            interstitial.OnAdImpressionRecorded += ImpressionRecorded;
            interstitial.OnAdClicked += AdClicked;
            interstitial.OnAdFullScreenContentOpened += AdFullScreenOpened;
            interstitial.OnAdFullScreenContentClosed += AdFullScreenClosed;
            interstitial.OnAdFullScreenContentFailed += AdFullScreenFailed;

            currentRetryInterstitial = 0;
        }


        /// <summary>
        /// Admob specific event triggered if an interstitial failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterstitialFailed(LoadAdError e)
        {
            // Gets the domain from which the error came.
            //string domain = loadAdError.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            //int code = loadAdError.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            //string message = loadAdError.GetMessage();

            // Gets the cause of the error, if available.
            //AdError underlyingError = loadAdError.GetCause();

            if (debug)
            {
                // All of this information is available via the error's toString() method.
                Debug.Log("Load error string: " + e.ToString());

                // Get response information, which may include results of mediation requests.
                ResponseInfo responseInfo = e.GetResponseInfo();
                Debug.Log("Response info: " + responseInfo.ToString());

                ScreenWriter.Write("Admob Interstitial Failed To Load ");
                ScreenWriter.Write("Admob Interstitial -> Load error string: " + e.ToString());
                ScreenWriter.Write("Admob Interstitial -> Response info: " + responseInfo.ToString());
            }

            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                if (debug)
                {
                    Debug.Log("Admob RETRY " + currentRetryInterstitial);
                    ScreenWriter.Write("Admob RETRY " + currentRetryInterstitial);
                }
                interstitialFailedToLoad = true;
            }
        }


        /// <summary>
        /// Admob specific event triggered after an interstitial was closed
        /// </summary>
        private void InterstitialClosed()
        {
            if (debug)
            {
                Debug.Log("Admob Reload Interstitial");
                ScreenWriter.Write("Admob Reload Interstitial");
            }

            //trigger complete event
            StartCoroutine(CompleteMethodInterstitial());

            //reload interstitial
            LoadInterstitial();
        }


        /// <summary>
        ///  Because Admob has some problems when used in multi threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodInterstitial()
        {
            yield return null;
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }
            if (OnInterstitialClosedWithAdvertiser != null)
            {
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Admob.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }
        }
        #endregion


        #region Interstitial Events
        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        /// <param name="adValue"></param>
        private void InterstitialAdPaied(AdValue adValue)
        {
            if (debug)
            {
                Debug.Log(String.Format("Admob Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
                ScreenWriter.Write(String.Format("Admob Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            }
        }


        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>        
        private void ImpressionRecorded()
        {
            if (debug)
            {
                Debug.Log("Admob Interstitial ad recorded an impression.");
                ScreenWriter.Write("Admob Interstitial ad recorded an impression.");
            }
        }


        /// <summary>
        ///Raised when a click is recorded for an ad. 
        /// </summary>
        private void AdClicked()
        {
            if (debug)
            {
                Debug.Log(this + "Interstitial ad was clicked.");
                ScreenWriter.Write(this + "Interstitial ad was clicked.");
            }
        }


        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        private void AdFullScreenOpened()
        {
            if (debug)
            {
                Debug.Log("Admob Interstitial ad full screen content opened.");
                ScreenWriter.Write("Admob Interstitial ad full screen content opened.");
            }
        }


        /// <summary>
        ///Raised when the ad closed full screen content. 
        /// </summary>
        private void AdFullScreenClosed()
        {
            if (debug)
            {
                Debug.Log(this + "Interstitial ad full screen content closed.");
                ScreenWriter.Write(this + "Interstitial ad full screen content closed.");
            }
            // Reload the ad so that we can show another as soon as possible.
            InterstitialClosed();
        }


        /// <summary>
        ///Raised when the ad failed to open full screen content. 
        /// </summary>
        /// <param name="error"></param>
        private void AdFullScreenFailed(AdError error)
        {
            if (debug)
            {
                Debug.LogError(this + "Interstitial ad failed to open full screen content. Error: " + error);
                ScreenWriter.Write(this + "Interstitial ad failed to open full screen content. Error: " + error);
            }
            // Reload the ad so that we can show another as soon as possible.
            InterstitialClosed();
        }
        #endregion


        #region Rewarded
        /// <summary>
        /// Check if Admob rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            if (rewardedVideo != null)
            {
                return rewardedVideo.CanShowAd();
            }
            return false;
        }


        /// <summary>
        /// Show Admob rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                watched = false;
                rewardedVideo.Show(RewardedVideoWatched);
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + "Rewarded ad cannot be shown.");
                    ScreenWriter.Write(this + "Rewarded ad cannot be shown.");
                }
            }
        }


        /// <summary>
        /// Show Admob rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                watched = false;
                rewardedVideo.Show(RewardedVideoWatched);

            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + "Rewarded ad cannot be shown.");
                    ScreenWriter.Write(this + "Rewarded ad cannot be shown.");
                }
            }
        }


        /// <summary>
        /// Loads an Admob rewarded video
        /// </summary>
        private void LoadRewardedVideo()
        {
            if (debug)
            {
                Debug.Log("Admob Start Loading Rewarded Video");
                ScreenWriter.Write("Admob Start Loading Rewarded Video");
            }

            // Clean up the old ad before loading a new one.
            if (rewardedVideo != null)
            {
                rewardedVideo.OnAdPaid -= RewardedPaid;
                rewardedVideo.OnAdImpressionRecorded -= RewardedImpressionRec;
                rewardedVideo.OnAdClicked -= RewardedAdClicked;
                rewardedVideo.OnAdFullScreenContentOpened -= RewardedFullScreenOpened;
                rewardedVideo.OnAdFullScreenContentClosed -= RewardedFullScreenClosed;
                rewardedVideo.OnAdFullScreenContentFailed -= RewardedFullScreenFailed;
                rewardedVideo.Destroy();
                rewardedVideo = null;
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder().Build();

            // send the request to load the ad.
            RewardedAd.Load(rewardedVideoId, adRequest, LoadRewardedVideoCallback);
        }


        private void LoadRewardedVideoCallback(RewardedAd ad, LoadAdError loadAdError)
        {
            // if error is not null, the load request failed.
            if (loadAdError != null || ad == null)
            {
                RewardedVideoFailed(loadAdError);
                return;
            }
            else
            {
                RewardedVideoLoaded(ad);
            }
            rewardedVideo = ad;
        }


        /// <summary>
        /// Admob specific event triggered after a rewarded video is loaded and ready to be watched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewardedVideoLoaded(RewardedAd ad)
        {
            rewardedVideo = ad;
            if (debug)
            {
                Debug.Log("Admob Rewarded Video Loaded.");
                ScreenWriter.Write("Admob Rewarded Video Loaded.");
            }

            rewardedVideo.OnAdPaid += RewardedPaid;
            rewardedVideo.OnAdImpressionRecorded += RewardedImpressionRec;
            rewardedVideo.OnAdClicked += RewardedAdClicked;
            rewardedVideo.OnAdFullScreenContentOpened += RewardedFullScreenOpened;
            rewardedVideo.OnAdFullScreenContentClosed += RewardedFullScreenClosed;
            rewardedVideo.OnAdFullScreenContentFailed += RewardedFullScreenFailed;

            currentRetryRewardedVideo = 0;
        }


        /// <summary>
        /// Admob specific event triggered if a rewarded video failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewardedVideoFailed(LoadAdError e)
        {
            LoadAdError loadAdError = e;

            // Gets the domain from which the error came.
            //string domain = loadAdError.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            //int code = loadAdError.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            //string message = loadAdError.GetMessage();

            // Gets the cause of the error, if available.
            //AdError underlyingError = loadAdError.GetCause();
            if (debug)
            {
                // All of this information is available via the error's toString() method.
                Debug.Log("Load error string: " + loadAdError.ToString());

                // Get response information, which may include results of mediation requests.
                ResponseInfo responseInfo = loadAdError.GetResponseInfo();
                Debug.Log("Response info: " + responseInfo.ToString());

                ScreenWriter.Write("Admob Rewarded Video Failed: " + loadAdError.ToString());
                Debug.Log("Admob Rewarded Video -> Load error string: " + loadAdError.ToString());
                Debug.Log("Admob Rewarded Video -> Response info: " + responseInfo.ToString());
            }

            //try again to load a rewarded video
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;
                if (debug)
                {
                    Debug.Log("Admob RETRY " + currentRetryRewardedVideo);
                    ScreenWriter.Write("Admob RETRY " + currentRetryRewardedVideo);
                }
                rewardedVideoFailedToLoad = true;
            }
        }


        private void RewardedVideoWatched(Reward reward)
        {
            if (debug)
            {
                Debug.Log(this + "Admob RewardedVideoWatched");
                ScreenWriter.Write(this + "Admob RewardedVideoWatched");
            }
            watched = true;
        }


        /// <summary>
        /// Admob specific event triggered when a rewarded video was skipped
        /// </summary>
        private void RewardedAdClosed()
        {
            if (debug)
            {
                Debug.Log("Admob OnAdClosed");
                ScreenWriter.Write("Admob OnAdClosed");
            }
            //trigger complete method
            StartCoroutine(CompleteMethodRewardedVideo(watched));

            //reload
            LoadRewardedVideo();
        }


        /// <summary>
        /// Because Admob has some problems when used in multi-threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodRewardedVideo(bool val)
        {
            yield return null;
            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(val);
                OnCompleteMethod = null;
            }
            if (OnCompleteMethodWithAdvertiser != null)
            {
                OnCompleteMethodWithAdvertiser(val, SupportedAdvertisers.Admob.ToString());
                OnCompleteMethodWithAdvertiser = null;
            }
        }
        #endregion


        #region Rewarded Ads Events
        /// <summary>
        ///Raised when an impression is recorded for an ad.
        /// </summary>
        private void RewardedImpressionRec()
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded ad recorded an impression.");
                ScreenWriter.Write("Admob Rewarded ad recorded an impression.");
            }
        }


        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        /// <param name="adValue"></param>
        private void RewardedPaid(AdValue adValue)
        {
            if (debug)
            {
                Debug.Log(String.Format("Admob Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
                ScreenWriter.Write(String.Format("Admob Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            }
        }


        /// <summary>
        ///Raised when a click is recorded for an ad.
        /// </summary>
        private void RewardedAdClicked()
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded ad was clicked.");
                ScreenWriter.Write("Admob Rewarded ad was clicked.");
            }
        }


        /// <summary>
        ///Raised when an ad opened full screen content. 
        /// </summary>
        private void RewardedFullScreenOpened()
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded ad full screen content opened.");
                ScreenWriter.Write("Admob Rewarded ad full screen content opened.");
            }
        }


        /// <summary>
        ///Raised when the ad closed full screen content.
        /// </summary>
        private void RewardedFullScreenClosed()
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded ad full screen content closed.");
                ScreenWriter.Write("Admob Rewarded ad full screen content closed.");
            }
            RewardedAdClosed();
        }


        /// <summary>
        ///Raised when the ad failed to open full screen content.
        /// </summary>
        /// <param name="error"></param>
        private void RewardedFullScreenFailed(AdError error)
        {
            if (debug)
            {
                Debug.LogError("Admob Rewarded ad failed to open full screen content. Error: " + error);
                ScreenWriter.Write("Admob Rewarded ad failed to open full screen content. Error: " + error);
            }
            //Reload the rewarded ad so that we can show another one as soon as possible
            RewardedAdClosed();
        }
        #endregion


        #region Banner
        /// <summary>
        /// Check if Admob banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }


        /// <summary>
        /// Show Admob banner
        /// </summary>
        /// <param name="position"> can be TOP or BOTTOM</param>
        ///  /// <param name="bannerType"> can be Banner or SmartBanner</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            bannerLoaded = false;
            bannerUsed = true;
            this.DisplayResult = DisplayResult;
            if (banner != null)
            {
                if (this.position == position && this.bannerType == bannerType && forceReload == false)
                {
                    if (debug)
                    {
                        Debug.Log("Admob Show banner");
                        ScreenWriter.Write("Admob Show Banner");
                    }
                    bannerLoaded = true;
                    banner.Show();
                    if (this.DisplayResult != null)
                    {
                        this.DisplayResult(true, position, bannerType);
                        this.DisplayResult = null;
                    }
                }
                else
                {
                    LoadBanner(position, bannerType);
                }
            }
            else
            {
                LoadBanner(position, bannerType);
            }
        }


        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        public void ResetBannerUsage()
        {
            bannerUsed = false;
        }


        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        /// <returns>true if current banner failed to load</returns>
        public bool BannerAlreadyUsed()
        {
            return bannerUsed;
        }


        /// <summary>
        /// Hides Admob banner
        /// </summary>
        public void HideBanner()
        {
            if (debug)
            {
                Debug.Log("Admob Hide banner");
                ScreenWriter.Write("Admob Hide banner");
            }
            if (banner != null)
            {
                if (bannerLoaded == false)
                {
                    //if banner is not yet loaded -> destroy so it cannot load later in the game
                    if (DisplayResult != null)
                    {
                        DisplayResult(false, position, bannerType);
                        DisplayResult = null;
                    }
                    banner.OnAdClicked -= BannerLoadSucces;
                    banner.OnBannerAdLoadFailed -= BannerLoadFailed;
                    banner.Destroy();
                    banner = null;
                    forceReload = true;
                }
                else
                {
                    //hide the banner -> will be available later without loading
                    banner.Hide();
                    forceReload = false;
                }
            }
        }


        /// <summary>
        /// Loads an Admob banner
        /// </summary>
        /// <param name="position">display position</param>
        /// <param name="bannerType">can be normal banner or smart banner</param>
        private void LoadBanner(BannerPosition position, BannerType bannerType)
        {
            if (debug)
            {
                Debug.Log("Admob  Start Loading Banner");
                ScreenWriter.Write("Admob Start Loading Banner");
            }

            //setup banner
            if (banner != null)
            {
                banner.OnBannerAdLoaded -= BannerLoadSucces;
                banner.OnBannerAdLoadFailed -= BannerLoadFailed;
                banner.OnAdPaid -= BannerAdPaied;
                banner.OnAdImpressionRecorded -= BannerImpressionRecorded;
                banner.OnAdClicked -= BannerClicked;
                banner.OnAdFullScreenContentOpened -= BannerFullScreenOpened;
                banner.OnAdFullScreenContentClosed -= BannerFullScreenClose;

                banner.Destroy();
                banner = null;
            }

            this.position = position;
            this.bannerType = bannerType;

            // create an instance of a banner view first.
            switch (position)
            {
                case BannerPosition.BOTTOM:
                    if (bannerType == BannerType.Banner)
                    {
                        // Create a 320x50 banner
                        banner = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
                    }
                    else
                    {
                        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                        banner = new BannerView(bannerId, adaptiveSize, AdPosition.Bottom);
                    }
                    break;
                case BannerPosition.TOP:
                    if (bannerType == BannerType.Banner)
                    {
                        banner = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);
                    }
                    else
                    {
                        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                        banner = new BannerView(bannerId, adaptiveSize, AdPosition.Top);
                    }
                    break;
            }

            // listen to events the banner may raise.
            banner.OnBannerAdLoaded += BannerLoadSucces;
            banner.OnBannerAdLoadFailed += BannerLoadFailed;
            banner.OnAdPaid += BannerAdPaied;
            banner.OnAdImpressionRecorded += BannerImpressionRecorded;
            banner.OnAdClicked += BannerClicked;
            banner.OnAdFullScreenContentOpened += BannerFullScreenOpened;
            banner.OnAdFullScreenContentClosed += BannerFullScreenClose;

            //request and load banner
            banner.LoadAd(CreateRequest());
        }
        #endregion


        #region Banner Events
        /// <summary>
        /// Admob specific event triggered after banner was loaded into the banner view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BannerLoadSucces()
        {
            if (debug)
            {
                Debug.Log("Admob Banner Loaded");
                ScreenWriter.Write("Admob Banner Loaded");
            }
            bannerLoaded = true;
            if (DisplayResult != null)
            {
                DisplayResult(true, position, bannerType);
                DisplayResult = null;
            }
        }


        /// <summary>
        /// Admob specific event triggered after banner failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="loadAdError"></param>
        private void BannerLoadFailed(LoadAdError loadAdError)
        {
            // Gets the domain from which the error came.
            //string domain = loadAdError.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            //int code = loadAdError.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            //string message = loadAdError.GetMessage();

            // Gets the cause of the error, if available.
            //AdError underlyingError = loadAdError.GetCause();
            if (debug)
            {
                // All of this information is available via the error's toString() method.
                Debug.Log("Admob Banner -> Load error string: " + loadAdError.ToString());

                // Get response information, which may include results of mediation requests.
                ResponseInfo responseInfo = loadAdError.GetResponseInfo();
                Debug.Log("Admob Banner -> Response info: " + responseInfo.ToString());
                ScreenWriter.Write("Admob Banner Failed To Load ");
                ScreenWriter.Write("Admob Banner -> Load error string: " + loadAdError.ToString());
                ScreenWriter.Write("Admob Banner -> Response info: " + responseInfo.ToString());
            }
            if (banner != null)
            {
                banner.OnBannerAdLoaded -= BannerLoadSucces;
                banner.OnBannerAdLoadFailed -= BannerLoadFailed;
                banner.Destroy();
            }
            banner = null;
            bannerLoaded = false;
            if (DisplayResult != null)
            {
                DisplayResult(false, position, bannerType);
                DisplayResult = null;
            }
        }


        /// <summary>
        /// Raised when the ad is estimated to have earned money.
        /// </summary>
        /// <param name="adValue"></param>
        private void BannerAdPaied(AdValue adValue)
        {
            if (debug)
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
                ScreenWriter.Write(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            }
        }


        /// <summary>
        /// Raised when an impression is recorded for an ad.
        /// </summary>
        private void BannerImpressionRecorded()
        {
            if (debug)
            {
                Debug.Log(this + "Banner view recorded an impression.");
                ScreenWriter.Write(this + "Banner view recorded an impression.");
            }
        }


        /// <summary>
        /// Raised when a click is recorded for an ad.
        /// </summary>
        private void BannerClicked()
        {
            if (debug)
            {
                Debug.Log(this + "Banner view was clicked.");
                ScreenWriter.Write(this + "Banner view was clicked.");
            }
        }


        /// <summary>
        /// Raised when an ad opened full screen content.
        /// </summary>
        private void BannerFullScreenOpened()
        {
            if (debug)
            {
                Debug.Log(this + "Banner view full screen content opened.");
                ScreenWriter.Write(this + "Banner view full screen content opened.");
            }
        }


        /// <summary>
        /// Raised when the ad closed full screen content.
        /// </summary>
        private void BannerFullScreenClose()
        {
            if (debug)
            {
                Debug.Log(this + "Banner view full screen content closed.");
                ScreenWriter.Write(this + "Banner view full screen content closed.");
            }
            //reload banner
            LoadBanner(position, bannerType);
        }
        #endregion


        /// <summary>
        /// Used to delay the admob events for multi-threading applications
        /// </summary>
        private void Update()
        {
            if (interstitialFailedToLoad)
            {
                interstitialFailedToLoad = false;
                Invoke("LoadInterstitial", reloadTime);
            }

            if (rewardedVideoFailedToLoad)
            {
                rewardedVideoFailedToLoad = false;
                Invoke("LoadRewardedVideo", reloadTime);
            }
        }


        /// <summary>
        /// Method triggered by Unity Engine when application is in focus.
        /// Because Admob uses multi-threading, there are some errors when you create coroutine outside the main thread so we want to make sure the app is on main thread.
        /// </summary>
        /// <param name="focus"></param>
        private void OnApplicationFocus(bool focus)
        {
            if (focus == true)
            {
                if (IsInterstitialAvailable() == false)
                {
                    if (currentRetryInterstitial == maxRetryCount)
                    {
                        LoadInterstitial();
                    }
                }

                if (IsRewardVideoAvailable() == false)
                {
                    if (currentRetryRewardedVideo == maxRetryCount)
                    {
                        LoadRewardedVideo();
                    }
                }
            }
        }
#else
            //dummy interface implementation, used when Admob is not enabled
            public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, System.Collections.Generic.List<PlatformSettings> platformSettings)
        {

        }

        public bool IsInterstitialAvailable()
        {
            return false;
        }

        public bool IsRewardVideoAvailable()
        {
            return false;
        }

        public void ShowInterstitial(UnityAction InterstitialClosed = null)
        {

        }

        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {

        }

        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {

        }

        public void HideBanner()
        {

        }

        public bool IsBannerAvailable()
        {
            return false;
        }

        public void ResetBannerUsage()
        {

        }

        public bool BannerAlreadyUsed()
        {
            return false;
        }

        public void ShowBanner(BannerPosition position, BannerType type, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            
        }

        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {

        }

        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {

        }

#endif
    }
}

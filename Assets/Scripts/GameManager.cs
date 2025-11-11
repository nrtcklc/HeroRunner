using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;




public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text coinText;
    public PlayerController playerController;
    public UIManager uiManager;
   

    private int currentLevel;



    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9001348990614170/6555471300";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-9001348990614170/5050817941";
#else
  private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;
    
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        else
        {
            instance = this;
        }
    }
    public void Start()
    {    
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartButton()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerScript = playerGO.GetComponent<PlayerController>();
        playerScript.GameStarted();
        uiManager.FirstTouch();

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            LoadInterstitialAd();
        });
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel >= PlayerPrefs.GetInt("unlockLevels"))
        {
            PlayerPrefs.SetInt("unlockLevels", currentLevel + 1);
        }
    }

    public void CurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }
    public void PauseGame()
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        Time.timeScale = 0.0f;
    }
    public void ContinueGame()
    {
        Camera.main.GetComponent<AudioListener>().enabled = true;
        Time.timeScale = 1.0f;
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (PlayerPrefs.GetInt("NoAds") == 0)
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
            Debug.LogError("Interstitial ad is not ready yet.");
            }
        }
    }
}

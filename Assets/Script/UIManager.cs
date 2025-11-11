using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public SoundManager soundManager;

    public UnityEngine.UI.Image whiteEffectImage;
    private int effectControl = 0;
    private bool radialShine;
    
    public Animator layoutAnimator;

    public Text coinText;

    //Buttonlar
    public GameObject settingsOpen;
    public GameObject settingsClose;
    public GameObject soundOn;
    public GameObject soundOff;
    public GameObject vibrationOn;
    public GameObject vibrationOff;
    public GameObject iap;
    public GameObject information;
    public GameObject player;
    public GameObject finishLine;
    public GameObject languageIcon;
    public GameObject languageMenu;
    public GameObject shopMenu;
    public GameObject informationMenu;
    public GameObject pauseIcon;
    public GameObject quitButton;

    public GameObject restartMenu;
    public GameObject pauseMenu;
    public GameObject home;
    public GameObject startButton;

    public GameObject touchToMove, noAds, shop, settings, layoutBackground, hearts;
    public GameObject finishScreen, blackBackround, complete, radialShineImage, coin, rewarded, passAd;

    public GameObject achievementCoin, nextLevel;
    public Text achievementText, coinTextFinished;

    private int currentLevel;

    public void Start()
    {
        if (PlayerPrefs.HasKey("Vibration") == false)
        {
            PlayerPrefs.SetInt("Vibration", 1);
        }
        if (PlayerPrefs.GetInt("NoAds") == 1)
        {
            NoAdsRemove();
        }
        CoinTextUpdate();
    }
    public void NoAdsRemove() 
    {
        noAds.SetActive(false); 
    }
    public void Update()
    {
        if (radialShine == true)
        {
            radialShineImage.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -15f * Time.deltaTime));
        }
    }
    public void FirstTouch()
    {
        startButton.SetActive(false);
        touchToMove.SetActive(false);
        noAds.SetActive(false);
        shop.SetActive(false);
        settingsOpen.SetActive(false);
        settingsClose.SetActive(false);
        layoutBackground.SetActive(false);
        soundOn.SetActive(false);
        soundOff.SetActive(false);
        vibrationOn.SetActive(false);
        vibrationOff.SetActive(false);
        iap.SetActive(false);
        information.SetActive(false);
        languageIcon.SetActive(false);
        languageMenu.SetActive(false);
        shopMenu.SetActive(false);
        informationMenu.SetActive(false);
        quitButton.SetActive(false);
        home.SetActive(false);
        pauseIcon.SetActive(true);
        hearts.SetActive(true);
    }
    public void CoinTextUpdate()
    {
        coinText.text = PlayerPrefs.GetInt("coinn").ToString();
    }
    public void FinishScreen()
    {
        CoinCalculator(100);
        CoinTextUpdate();
        StartCoroutine("FinishLaunch");
    }
    public IEnumerator FinishLaunch()
    {   
        pauseIcon.SetActive(false);
        radialShine = true;
        finishScreen.SetActive(true);
        blackBackround.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        complete.SetActive(true);
        soundManager.CompleteSound();
        yield return new WaitForSeconds(0.1f);
        radialShineImage.SetActive(true);
        coin.SetActive(true);
        soundManager.CompleteSound();
        yield return new WaitForSeconds(1f);
        rewarded.SetActive(true);
        soundManager.CompleteSound();
        yield return new WaitForSeconds(2.0f);
        passAd.SetActive(true);
    }
    public void RewardedButton()
    {
        StartCoroutine("AfterRewardButton");
    }
    public IEnumerator AfterRewardButton()
    {
        passAd.SetActive(false);
        coinTextFinished.gameObject.SetActive(false);
        rewarded.SetActive(false);      
        achievementCoin.SetActive(true);
        achievementText.gameObject.SetActive(true);
        CoinCalculator(100);
        for (int i = 100; i < 201; i+=1)
        {
            achievementText.text = "+" + i.ToString();
            coinText.text = PlayerPrefs.GetInt("coinn").ToString();
            yield return new WaitForSeconds(0.001f);      
        }    
        yield return new WaitForSeconds(0.1f); 
        nextLevel.SetActive(true);
    }
    public void FailPanel()
    {
      restartMenu.SetActive(true);
      quitButton.SetActive(true);
      pauseIcon.SetActive(false);
    }
public void SettingsOpen()
    {
        settingsOpen.SetActive(false);
        settingsClose.SetActive(true);
        layoutAnimator.SetTrigger("SlideIn");

        if (PlayerPrefs.GetInt("Vibration") == 1)
        {
            vibrationOn.SetActive(true);
            vibrationOff.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Vibration") == 2)
        {
            vibrationOn.SetActive(false);
            vibrationOff.SetActive(true);
        }
    }
    public void SettingsClose()
    {
        layoutAnimator.SetTrigger("SlideOut");
        settingsClose.SetActive(false);
        settingsOpen.SetActive(true);
    }
    public void VibrationOn()
    {
        vibrationOn.SetActive(false);
        vibrationOff.SetActive(true);
        PlayerPrefs.SetInt("Vibration", 2);
    }
    public void VibrationOff()
    {
        vibrationOff.SetActive(false);
        vibrationOn.SetActive(true);
        PlayerPrefs.SetInt("Vibration", 1);
    } 
    public void PrivacyPolicy()
    {
        Application.OpenURL("https://docs.google.com/document/d/14cr5XIC-qVY4K5JQFn922fuLfzwxp8Vx/edit?usp=drive_link&ouid=115843884881959542974&rtpof=true&sd=true");
    }
    public void TermOfUse()
    {
        Application.OpenURL("https://docs.google.com/document/d/1nGZ3jWRIr2h97mD9MYoNlT4hDS9xrAbU/edit?usp=drive_link&ouid=115843884881959542974&rtpof=true&sd=true");
    }
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/nrtcklc");
    }
    public void Instagram()
    {
        Application.OpenURL("https://www.instagram.com/nrtcklc/");
    }
    public void Twitter()
    {
        Application.OpenURL("https://twitter.com/nrtcklc");
    }
    public void VKontakte()
    {
        Application.OpenURL("https://vk.com/nrtcklc");
    }
    public void Telegram()
    {
        Application.OpenURL("https://t.me/nrtcklc");
    }
    public void Skype()
    {
        Application.OpenURL("https://join.skype.com/invite/nxQe1Tgz3PFE");
    }
    public IEnumerator WhiteEffect()
    {
        whiteEffectImage.gameObject.SetActive(true);
        while (effectControl == 0)
        {
            yield return new WaitForSeconds(0.1f);
            whiteEffectImage.color += new Color(0, 0, 0, 1f);

            if (whiteEffectImage.color == new Color(whiteEffectImage.color.r, whiteEffectImage.color.g, whiteEffectImage.color.b, 1))
            {
                effectControl = 1;
            }
        }
        while (effectControl == 1)
        {
            yield return new WaitForSeconds(0.1f);
            whiteEffectImage.color -= new Color(0, 0, 0, 1f);

            if (whiteEffectImage.color == new Color(whiteEffectImage.color.r, whiteEffectImage.color.g, whiteEffectImage.color.b, 0))
            {
                effectControl = 2;
            }
        }
        if (effectControl == 2)
        {
            Debug.Log("Effect Bitti.");
        }
    }
    public void CoinCalculator(int coin)
    {
        if (PlayerPrefs.HasKey("coinn"))
        {
            int oldScore = PlayerPrefs.GetInt("coinn");
            PlayerPrefs.SetInt("coinn", oldScore + coin);
        }
        else
            PlayerPrefs.SetInt("coinn", 100);
    }
}

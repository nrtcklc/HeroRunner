using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    float playerSpeed = 6f;
    float xSpeed;
    float maxX = 10f;
    bool isPlayerMoving;
    public GameObject headBoxGO;
    private ScaleCalculator ScaleCalculator;
    Renderer headBoxRenderer;
    private Material currentHeadMat;
    public Material warningMat;
    public GameObject spine;
    public GameObject finishLine;

    private Touch touch;
    public bool firstTouchControl = false;
    public UIManager uiManager;
    public GameManager gameManager;

    Animator anim;


    public AudioSource playerAudioSource;
    public AudioClip gateClip, colorBoxClip, ObstacleClip, sucessClip;

    //Oyunun baþarýsýz olmasý durumunda olacaklar
    public AudioClip failedClip, hitClip;
    //Canvasda can simegesinin azalmasý
    private int canSayma = 3;
    [SerializeField] private GameObject[] kalpIcon;

    //Odüllü Reklam yüklemesi.
    public RewaredAds rewaredAds;

    //karakterin haraketlerinin sýnýrlandýrýlmasý.
    public GameObject vectorBack;
    public GameObject vectorForward;


    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9001348990614170/5298229953";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-9001348990614170/7676981280";
#else
  private string _adUnitId = "unused";
#endif

    BannerView _bannerView;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        ScaleCalculator = new ScaleCalculator();
        headBoxRenderer = headBoxGO.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        currentHeadMat = headBoxRenderer.material;
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            if (PlayerPrefs.HasKey("NoAds") == false)
            {
                PlayerPrefs.SetInt("NoAds", 0);
            }
            if (PlayerPrefs.GetInt("NoAds") == 0)
            {
                LoadAd();
            }
        });
        anim.SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update()
    {

        if (isPlayerMoving == false)
        {
            return;
        }

        float touchX = 0;
        float newXValue;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            xSpeed = 250f;
            touchX = Input.GetTouch(0).deltaPosition.x / Screen.width;

        }

        else if (Input.GetMouseButton(0))
        {
            xSpeed = 500f;
            touchX = Input.GetAxis("Mouse X");
        }
        newXValue = transform.position.x + xSpeed * touchX * Time.deltaTime;
        newXValue = Mathf.Clamp(newXValue, -maxX, maxX);
        Vector3 playerNewPosition = new Vector3(newXValue, transform.position.y, transform.position.z + playerSpeed * Time.deltaTime);
        transform.position = playerNewPosition;
        vectorBack.transform.position += new Vector3(0, 0, playerSpeed * Time.deltaTime);
        vectorForward.transform.position += new Vector3(0, 0, playerSpeed * Time.deltaTime);

        if (canSayma == 0)
        {
            canSayma = 3;
            Camera.main.GetComponent<AudioSource>().Stop();
            //PlayAudio(failedClip, 1);
            playerAudioSource.PlayOneShot(failedClip);
            anim.SetBool("Run", false);
            anim.SetBool("Die", true);
            uiManager.FailPanel();        
            spine.SetActive(false);
            playerSpeed = 0f;
            StartCoroutine( SoundOff());
    
        }
    }
    IEnumerator SoundOff()
    {
        yield return new WaitForSeconds(2.5f);
        Camera.main.GetComponent<AudioListener>().enabled = false;  
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FinishLine")
        {
            isPlayerMoving = false;
            finishLine.GetComponent<CapsuleCollider>().enabled = false;
            StopBackgroundMusic();
            //PlayAudio(sucessClip, 1f);
            playerAudioSource.PlayOneShot(sucessClip);
            rewaredAds.LoadRewardedAd();
            gameManager.LoadInterstitialAd();
            uiManager.FinishScreen();
            spine.SetActive(false);
            // stop moving
            anim.SetBool("Run", false);
            anim.SetBool("TurnBack", true);
            StartCoroutine(Victory());

        }
        if (other.tag == "enemy")
        {          
            canSayma--;
            kalpIcon[canSayma].gameObject.SetActive(false);
            PlayAudio(hitClip, 01);

            if (PlayerPrefs.GetInt("Vibration") == 1)
            {
                Vibration.Vibrate(50);
            }
            else if (PlayerPrefs.GetInt("Vibration") == 2)
            {
                Debug.Log("Vibration Off");
            }
        }
    }
    IEnumerator Victory()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Victory", true);
        anim.SetBool("TurnBack", false);
    }

    public void PassedGate(GateType gateType, int gateValue)
    {
        headBoxGO.transform.localScale = ScaleCalculator.CalculatePlayerHeadSize(gateType, gateValue, headBoxGO.transform);
        PlayAudio(gateClip, 0.6f);
        // we will change size of HeadBox according to Gate type and value
    }

    public void TouchedToColorBox(Material boxMat)
    {
        headBoxRenderer.material = boxMat;
        currentHeadMat = boxMat;
        PlayAudio(colorBoxClip, 1f);

        // color ninja's headbox
    }

    public void TouchedToObstacle()
    {
        headBoxGO.transform.localScale = ScaleCalculator.DecreasePlayerHeadSize(headBoxGO.transform);
        StartCoroutine(StartRedBlinkEffect());
        PlayAudio(ObstacleClip, 0.6f);
        // decrase the head.
    }

    private IEnumerator StartRedBlinkEffect()
    {
        headBoxGO.transform.GetChild(0).GetComponent<MeshRenderer>().material = warningMat;
        // color head in red
        yield return new WaitForSeconds(0.3f);
        headBoxGO.transform.GetChild(0).GetComponent<MeshRenderer>().material = currentHeadMat;
        // go back to current color
    }

    public void GameStarted()
    {
        isPlayerMoving = true;

    }

    private void PlayAudio(AudioClip audio, float volume)
    {
        playerAudioSource.PlayOneShot(audio, volume);
    }

    private void StopBackgroundMusic()
    {
        Camera.main.GetComponent<AudioSource>().Stop();
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
    }

    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    public void SuccesSound()
    {
        PlayAudio(sucessClip, 1f);
    }
}


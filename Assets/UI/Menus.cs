using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//All code relating to the menus the player can access before the game starts including the title scene,
//settings scene, shop scene, and exit scene
public class Menus : MonoBehaviour
{
    public static bool m_playOnAwake = false; //If true when the scene starts, then game starts automatically

    [Header("Title Screen")]
    [SerializeField] RectTransform m_titleScreen;
    [SerializeField] TMPro.TMP_Text m_highScoreText;

    [Header("Settings Screen")]
    [SerializeField] RectTransform m_settingsScreen;
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;

    [Header("ShopScreen")]
    public ShopItemData[] m_shopItems;
    [SerializeField] RectTransform m_shopScreen;
    [SerializeField] ToggleGroup m_shopTabsToggleGroup;
    [SerializeField] RectTransform m_shopContent;
    [SerializeField] TMPro.TMP_Text m_shopCarrotText;

    void Awake()
    {
        //load shop data
        foreach (var i in FindObjectsOfType<ShopItem>(true))
        {
            i.m_Data.Init();
            i.m_Data.Load();
        }
    }

    void Start()
    {
        //Start the game automatically when playOnAwake starts
        if (m_playOnAwake) StartGame();
        else
        {
            //Set the high score text on the title screen
            int highScore = SaveSystem.m_data.m_highScore;
            if (highScore > 0) m_highScoreText.text = "High Score: " + highScore.ToString();
            else m_highScoreText.gameObject.SetActive(false);

            //Set settings in settings menu
            if (GameManager.m_applicationStarted)
            {
                m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_data.m_sfxVolume);
                m_audioMixer.SetFloat("musicVolume", SaveSystem.m_data.m_musicVolume);
                QualitySettings.SetQualityLevel(SaveSystem.m_data.m_qualityLevel);
            }

            {
                float sfxVolumeValue; m_audioMixer.GetFloat("sfxVolume", out sfxVolumeValue);
                m_sfxVolumeSlider.value = sfxVolumeValue;
            }
            {
                float musicVolumeValue; m_audioMixer.GetFloat("musicVolume", out musicVolumeValue);
                m_musicVolumeSlider.value = musicVolumeValue;
            }
            m_graphicsDropdown.value = QualitySettings.GetQualityLevel();

            //Set number of carrots collected to text in shop
            m_shopCarrotText.text = SaveSystem.m_data.m_carrots.ToString();
        }
    }

    void Update()
    {
        //Start the game when the player presses the jump button
        if
        (
            InputUtilities.CanUseJumpInput() && //Check whether the player presses the jump input
            m_titleScreen.gameObject.activeSelf //Check whether the title screen is open
        )
        {
            StartGame();
            return;
        }
    }

    void StartGame()
    {
        //Dont automatically start the game the next time the player plays
        m_playOnAwake = false;
        
        //Activate the game manager and start the game
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.gameObject.SetActive(true);
        gameManager.StartGame();

        //Destroy Menus
        Destroy(gameObject);
    }

    #region Settings Menu
    public void SetSFXVolume(float _value)
    {
        m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_data.m_sfxVolume = _value);
    }

    public void SetMusicVolume(float _value)
    {
        m_audioMixer.SetFloat("musicVolume", SaveSystem.m_data.m_musicVolume = _value);
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(SaveSystem.m_data.m_qualityLevel = _qualityIndex);
    }
    #endregion

    #region Shop Menu
    public void OnShopTabChange(int _tabIndex)
    {
        //Set whether the tab is interactable
        foreach(Transform child in m_shopTabsToggleGroup.transform)
        {
            if (child.GetSiblingIndex() == _tabIndex) child.GetComponent<Toggle>().interactable = false;
            else child.GetComponent<Toggle>().interactable = true;
        }

        //Open the coresponding shop menu
        foreach (Transform child in m_shopContent.transform)
        {
            if (child.GetSiblingIndex() == _tabIndex) child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Credits Menu
    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
    #endregion Credits Menu

    #region Exit Menu
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}

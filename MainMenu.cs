using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup MainMenuPanel;
    public CanvasGroup MapSelectionPanel;
    public CanvasGroup InstructionPanel;
    public CanvasGroup SettingsPanel;
    public CanvasGroup LoadingPanel;

    public Dropdown resDrop;
    Resolution[] resolutions;

    public Toggle fullscreenToggle;
    public Dropdown qualityDrop;

    public Slider[] volumeSlider;
    private AudioSourceManager[] audioObjects;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuPanel = GameObject.Find("MainMenuPanel").GetComponent<CanvasGroup>();
        MapSelectionPanel = GameObject.Find("MapSelectionPanel").GetComponent<CanvasGroup>();
        InstructionPanel = GameObject.Find("InstructionsPanel").GetComponent<CanvasGroup>();
        SettingsPanel = GameObject.Find("SettingsPanel").GetComponent<CanvasGroup>();
        LoadingPanel = GameObject.Find("LoadingPanel").GetComponent<CanvasGroup>();
        audioObjects = FindObjectsOfType<AudioSourceManager>();
        //-------------------------------------------
        resolutions = Screen.resolutions;

        resDrop.ClearOptions();

        List<string> options = new List<string>();

        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentRes = i;
            }
        }

        for (int i = 0; i < volumeSlider.Length; i++)
        {
            volumeSlider[i].value = AudioManager.Instance.levels[i];
        }

        resDrop.AddOptions(options);
        resDrop.value = currentRes;
        resDrop.RefreshShownValue();

        qualityDrop.value = QualitySettings.GetQualityLevel();
        qualityDrop.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
        AudioManager.Instance.PlayMusic("RTS_music", true);
        AudioManager.Instance.StopAtmosphereSounds();
        AudioManager.Instance.PlayAtmosphereSound("Nature2");
    }

    public void StartGame()
    {
        MainMenuPanel.alpha = 0;
        MainMenuPanel.interactable = false;
        MainMenuPanel.blocksRaycasts = false;

        MapSelectionPanel.alpha = 1;
        MapSelectionPanel.blocksRaycasts = true;
        MapSelectionPanel.interactable = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SelectMap1()
    {
        ShowLoadingScreen();
        GameManager.Instance.LoadMap(2);
    }

    public void SelectMap2()
    {
        ShowLoadingScreen();
        GameManager.Instance.LoadMap(3);
    }

    public void SelectMap3()
    {
        ShowLoadingScreen();
        GameManager.Instance.LoadMap(4);
    }

    public void SelectMap4()
    {
        ShowLoadingScreen();
        GameManager.Instance.LoadMap(Random.Range(2, 5));
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ShowInstructions()
    {
        MainMenuPanel.alpha = 0;
        MainMenuPanel.interactable = false;
        MainMenuPanel.blocksRaycasts = false;

        InstructionPanel.alpha = 1;
        InstructionPanel.blocksRaycasts = true;
        InstructionPanel.interactable = true;
    }

    public void HideInstruction()
    {
        MainMenuPanel.alpha = 1;
        MainMenuPanel.interactable = true;
        MainMenuPanel.blocksRaycasts = true;

        InstructionPanel.alpha = 0;
        InstructionPanel.blocksRaycasts = false;
        InstructionPanel.interactable = false;
    }

    public void ShowSettings()
    {
        MainMenuPanel.alpha = 0;
        MainMenuPanel.interactable = false;
        MainMenuPanel.blocksRaycasts = false;

        SettingsPanel.alpha = 1;
        SettingsPanel.blocksRaycasts = true;
        SettingsPanel.interactable = true;
    }

    public void HideSettings()
    {
        MainMenuPanel.alpha = 1;
        MainMenuPanel.interactable = true;
        MainMenuPanel.blocksRaycasts = true;

        SettingsPanel.alpha = 0;
        SettingsPanel.blocksRaycasts = false;
        SettingsPanel.interactable = false;
    }

    public void ShowLoadingScreen()
    {
        LoadingPanel.alpha = 1;
        LoadingPanel.interactable = true;
        LoadingPanel.blocksRaycasts = true;
    }

    public void SetMusicVolume(float volume)
    {
        if (volumeSlider[0].value != AudioManager.Instance.levels[0])
        {
            AudioManager.Instance.levels[0] = volume;
            AudioManager.Instance.ApplyAudioLevels(0, false);
        }
    }

    public void SetMessageVolume(float volume)
    {
        if (volumeSlider[1].value != AudioManager.Instance.levels[1])
        {
            AudioManager.Instance.levels[1] = volume;
            AudioManager.Instance.ApplyAudioLevels(1, false);
        }
    }

    public void SetAmbientVolume(float volume)
    {
        if (volumeSlider[2].value != AudioManager.Instance.levels[2])
        {
            AudioManager.Instance.levels[2] = volume;
            AudioManager.Instance.ApplyAudioLevels(2, false);

            foreach(AudioSourceManager source in audioObjects)
            {
                if(source != null)
                {
                    source.SetVolume(AudioManager.Instance.levels[2]);
                }
            }
        }
    }

}

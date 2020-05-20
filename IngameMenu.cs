using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngameMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public CanvasGroup MenuPanel;
    public CanvasGroup FirstStepsPanel;
    public CanvasGroup ScorePanel;
    private ResourceManager RM;
    public Button ResumeButton;
    public ButtonSystem BM;
    private Text message;
    private float[] audioLevel;

    // Start is called before the first frame update
    void Start()
    {
        audioLevel = new float[AudioManager.Instance.levels.Length];

        StartCoroutine(MenuPopup());
        MenuPanel = GameObject.Find("MenuPanel").GetComponent<CanvasGroup>();
        FirstStepsPanel = GameObject.Find("FirstStepsPanel").GetComponent<CanvasGroup>();
        ScorePanel = GameObject.Find("ScorePanel").GetComponent<CanvasGroup>();
        ResumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
        BM = FindObjectOfType<ButtonSystem>();
        message = GameObject.Find("Message").GetComponent<Text>();

        RM = FindObjectOfType<ResourceManager>();

        for (int i = 0; i < AudioManager.Instance.levels.Length; i++)
        {
            audioLevel[i] = AudioManager.Instance.levels[i];
        }

        AudioManager.Instance.PlayAtmosphereSound("Nature1");
        AudioManager.Instance.PlayAtmosphereSound("Nature2");
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!AudioManager.Instance.musicMute)
            {
                for (int i = 0; i < 3; i++)
                {
                    AudioManager.Instance.ApplyAudioLevels(i, true);
                }
                
            }
            else
            {
                for (int i = 0; i < AudioManager.Instance.levels.Length; i++)
                {
                    AudioManager.Instance.levels[i] = audioLevel[i];
                }

                for (int i = 0; i < 3; i++)
                {
                    AudioManager.Instance.ApplyAudioLevels(i, false);
                }
            }
        }
    }

    public void Pause()
    {
        if (!GameManager.Instance.WinCondition() && !GameManager.Instance.LoseCondition())
        {
            if (isPaused)
            {
                MenuPanel.alpha = 0;
                MenuPanel.blocksRaycasts = false;
                MenuPanel.interactable = false;

                Time.timeScale = 1f;
                isPaused = false;

                if (!BuildingPlacement.buildingProcess)
                {
                    BM.UnlockButtons();
                }
            }
            else
            {
                MenuPanel.alpha = 1;
                MenuPanel.blocksRaycasts = true;
                MenuPanel.interactable = true;
                message.text = "GAME PAUSED!";

                Time.timeScale = 0f;
                isPaused = true;
                BM.LockButtons();
            }
        }
    }

    public void LoadMainMenu()
    {
        isPaused = false;
        GameManager.Instance.LoadMap(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Confirm()
    {
        FirstStepsPanel.alpha = 0;
        FirstStepsPanel.blocksRaycasts = false;
        FirstStepsPanel.interactable = false;
    }

    public void CloseOverview()
    {
        ScorePanel.alpha = 0;
        ScorePanel.blocksRaycasts = false;
        ScorePanel.interactable = false;
    }

    IEnumerator MenuPopup()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);

            if (GameManager.Instance.WinCondition())
            {
                RM.titleDisplay.text = "VICTORY!";
                RM.ScoresOverview();
                //-------MENU----------------
                MenuPanel.alpha = 1;
                MenuPanel.blocksRaycasts = true;
                MenuPanel.interactable = true;
                //------SCORES---------------
                ScorePanel.alpha = 1;
                ScorePanel.blocksRaycasts = true;
                ScorePanel.interactable = true;
                //---------------------------
                message.text = "VICTORY!";
                ResumeButton.onClick.RemoveAllListeners();
                ResumeButton.GetComponentInChildren<Text>().text = "NEXT LEVEL";
                int mapIndex = GameManager.Instance.mapIndex;
                if (mapIndex < 4)
                {
                    mapIndex++;

                    ResumeButton.onClick.AddListener(() => GameManager.Instance.LoadMap(mapIndex));
                }
                else
                {
                    ResumeButton.interactable = false;
                }

                Time.timeScale = 0f;
                isPaused = true;
                BM.LockButtons();
                AudioManager.Instance.PlayMessageSound("Victory");
            }

            if(GameManager.Instance.LoseCondition())
            {
                RM.titleDisplay.text = "DEFEAT!";
                RM.ScoresOverview();
                //-------MENU----------------
                MenuPanel.alpha = 1;
                MenuPanel.blocksRaycasts = true;
                MenuPanel.interactable = true;
                //------SCORES---------------
                ScorePanel.alpha = 1;
                ScorePanel.blocksRaycasts = true;
                ScorePanel.interactable = true;
                //---------------------------
                message.text = "GAME OVER!";
                ResumeButton.interactable = false;

                Time.timeScale = 0f;
                isPaused = true;
                BM.LockButtons();
                AudioManager.Instance.PlayMessageSound("Defeat");
            }
        }
    }
}

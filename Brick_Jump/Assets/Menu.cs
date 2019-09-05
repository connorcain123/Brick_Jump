using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject startPanel, settingsPanel, pausePanel, gameOverPanel, gameWinPanel, gameModePanel, levelSelectPanel;
    public Button levelButton;
    public GameManagerScript gm;
    public Slider musicSlider, soundFXSlider;
    public Button musicSoundButton, soundFXButton, pauseButton;
    public Sprite soundOn, soundOff;
    private GameObject prevOpenMenu;
    private int levelUnlocked, totalLevels;
    private float prevMusicVolume, prevSoundFXVolume;

    private void Start()
    {
        totalLevels = (SceneManager.sceneCountInBuildSettings) - 2;

        levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);
        musicSlider.value = PlayerPrefs.GetFloat("MusicSlider", 100);
        soundFXSlider.value = PlayerPrefs.GetFloat("SoundFXSlider", 100);

        if(SceneManager.GetActiveScene().name == "Start")
        {
            for (int i = 1; i <= totalLevels; i++)
            {
                Button button = (Button)Instantiate(levelButton);
                button.GetComponentInChildren<TMP_Text>().text = i.ToString();
                button.transform.SetParent(levelSelectPanel.transform, false);
                string levelName = "Level" + i.ToString(); 
                button.onClick.AddListener(delegate { LoadScene(levelName); });
                button.transform.localPosition = new Vector3(-600 + (200 * i), 125, 0);

                for(int j = 0; j < PlayerPrefs.GetInt(levelName + "Stars", 0); j++)
                {
                    button.transform.GetChild(j + 1).GetComponent<SpriteRenderer>().color = Color.white;
                }

                if(i > levelUnlocked)
                {
                    button.interactable = false;
                }
            }
        }    
    }

    public void Update()
    {
        if(GameObject.Find("PauseButton") != null)
        {
            if(gm.GameOver || gm.LevelComplete)
            {
                pauseButton.enabled = false;
            }
          
            if (!pauseButton.enabled)
            {
                pauseButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                pauseButton.GetComponent<Image>().color = Color.white;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().name == "Start")
            {
                GameObject openMenu = null;

                foreach (Transform transform in GameObject.Find("Canvas").transform)
                {
                    if (transform.gameObject.activeSelf)
                    {
                         openMenu = transform.gameObject;
                    }
                }
                if(openMenu.name == "StartMenuPanel")
                {
                    Application.Quit();
                }
                else
                {
                    switch (openMenu.name)
                    {
                        case "GameModePanel":
                            OpenMenu(startPanel);
                            CloseMenu(openMenu);
                            break;
                        default:
                            OpenPrevMenu();
                            CloseMenu(openMenu);
                            break;
                    }                                   
                }
            }
            else
            {
                if (!pausePanel.activeSelf && !gameOverPanel.activeSelf && !gameWinPanel.activeSelf)
                {
                    PauseButton();
                }
                else
                {
                    LoadScene("Start");
                }
                
            }
        }

        if (gm != null)
        {
            if (gm.GameOver && !settingsPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
            }

            if (gm.LevelComplete && !settingsPanel.activeSelf)
            {
                int nextLevelNum = int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")) + 1;
                if (nextLevelNum > totalLevels)
                {
                    gameWinPanel.transform.Find("NextLevelButton").GetComponent<Button>().interactable = false;
                }
                gameWinPanel.SetActive(true);
            }
        }      
    }

    public void OpenMenu(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void OpenPrevMenu()
    {
        prevOpenMenu.SetActive(true);
    }

    public void CloseMenu(GameObject panel)
    {
        prevOpenMenu = panel;
        panel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if(sceneName == "Level")
        {
            sceneName += (int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")) + 1).ToString();
        }
        SceneManager.LoadScene(sceneName);
    }

    public void PauseButton()
    {
        if (pausePanel.activeSelf)
        {
            gm.ResumeGame();
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundButton(GameObject slider)
    {
        if (slider.GetComponent<Slider>().value == 0)
        {
            if(slider.name == "MusicSlider")
            {
                slider.GetComponent<Slider>().value = prevMusicVolume;
            }
            else
            {
                slider.GetComponent<Slider>().value = prevSoundFXVolume;
            }
            
            if (slider.GetComponent<Slider>().value == 0)
            {
                slider.GetComponent<Slider>().value = 100;
            }
            slider.GetComponentInChildren<Button>().GetComponent<Image>().sprite = soundOn;
        }
        else
        {
            SavePrevVolume(slider);
            slider.GetComponent<Slider>().value = 0;
            slider.GetComponentInChildren<Button>().GetComponent<Image>().sprite = soundOff;
        }
    }

    public void SavePrevVolume(GameObject slider)
    {
        if(slider.name == "MusicSlider")
        {
            prevMusicVolume = slider.GetComponent<Slider>().value;
        }
        else
        {
            prevSoundFXVolume = slider.GetComponent<Slider>().value;     
        }
    }

    public void UpdateSliderVolume(GameObject slider)
    {
        if(musicSlider.value == 0)
        {
            slider.GetComponentInChildren<Button>().GetComponent<Image>().sprite = soundOff;
        }
        else
        {
            slider.GetComponentInChildren<Button>().GetComponent<Image>().sprite = soundOn;
        }
        PlayerPrefs.SetFloat(slider.name, musicSlider.value);
        PlayerPrefs.Save();
    }


    


}

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject levelSelectPanel;
    public GameObject mainMenuPanel;
    public Canvas menuCanvas;

    public void Play()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void Options()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void LoadLevel(string levelName)
    {
        if (SceneTransition.instance != null)
        {
            /*if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (menuCanvas != null) menuCanvas.enabled = false;*/
            SceneTransition.instance.FadeToScene(levelName);
        }
        else
        {
            Debug.LogError("SceneTransition instance not found!");
        }
    }
}

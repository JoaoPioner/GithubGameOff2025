using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    private Button _restartButton;
    
    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private TextMeshProUGUI _gameOverText;

    protected void Awake()
    {
        _restartButton.onClick.AddListener(Restart);
        _exitButton.onClick.AddListener(ExitToMenu);
    }
    
    
    public void Open(bool wonGame)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;

        if (wonGame)
        {
            _gameOverText.text = "Victory!";
        }
        else
        {
            _gameOverText.text = "Defeat!";
        }
    }

    private void Restart()
    {
        SceneTransition.instance.FadeToScene(SceneManager.GetActiveScene().name);
    }

    private void ExitToMenu()
    {
        SceneTransition.instance.FadeToScene("MainMenuScene");
    }
}

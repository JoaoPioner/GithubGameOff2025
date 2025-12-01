using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    [SerializeField]
    private GameOverScreen _gameOverScreen;

    [SerializeField]
    private GameObject _hudObject;

    private void Start()
    {
        _gameOverScreen.gameObject.SetActive(false);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameEnded += OpenGameOverScreen;
        }
    }

    private void OpenGameOverScreen(bool wonGame)
    {
        Debug.Log("<color=lime>Opening game over screen</color>");
        DisableHUD();
        _gameOverScreen.Open(wonGame);
    }

    private void DisableHUD()
    {
        _hudObject.gameObject.SetActive(false);
    }
}

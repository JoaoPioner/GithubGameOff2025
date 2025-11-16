using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance { get; private set; }

    [Header("Economia")]
    public int gold = 100;

    [Header("Progresso da Partida")]
    public int actualRound = 1; // A rodada/onda atual
    public int VPCount;

    [Header("Condições de Jogo")]
    public bool jogoEmPausa = false;
    public bool jogoAcabou = false;

    private float tickTimer = 0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= 1f)
        {
            AddGold(10);
            Debug.Log("Ouro atual: " + gold);
            tickTimer = 0f;
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        else
        {
            Debug.Log("Ouro insuficiente!");
            return false;
        }
    }

    public void PauseGame()
    {
        jogoEmPausa = true;
        Time.timeScale = 0f; // Pausa o tempo do jogo
    }

    public void EndGame(bool playerWon)
    {
        jogoAcabou = true;
        if (playerWon)
        {
            Debug.Log("Parabéns! Você venceu!");
        }
        else
        {
            Debug.Log("Game Over! Você perdeu!");
        }
        // Aqui você pode adicionar lógica para mostrar telas de vitória/derrota, reiniciar o jogo, etc.
    }

}

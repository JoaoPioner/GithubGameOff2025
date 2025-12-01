using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
  private static GameStateManager _instance;
  public static GameStateManager Instance { get; private set; }

  [Header("Economia")]
  public int gold = 100;
  public int maxGold = 500;
  public int goldPerTick = 10;
  public float tickInterval = 5f;

  [Header("Progresso da Partida")]
  public int actualRound = 1;
  public int VPCount;

  [Header("Condições de Jogo")]
  public bool jogoEmPausa = false;
  public bool jogoAcabou = false;

  private float tickTimer = 0f;

  public event Action<int> OnGoldChanged;
  public event Action<bool> OnGameEnded;

  public event Action<int> onRoundChanged;

  [SerializeField]
  private WaveManager waveManager;

  public int MaxRounds  => waveManager.MaxRounds;
  
  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Debug.Log("GameManager Duplicado Destruído");
      Destroy(gameObject);
    }

    if (waveManager != null)
    {
      waveManager.OnLastWaveFinished += () => EndGame(true);
    }
  }

  public void VPDestroyed()
  {
    VPCount--;
    Debug.Log("Torre destruída! VPs restantes: " + VPCount);
    if (VPCount <= 0)
    {
      EndGame(false);
    }
  }

  void Update()
  {
    tickTimer += Time.deltaTime;
    if (!jogoEmPausa && !jogoAcabou && tickTimer >= tickInterval)
    {
      AddGold(goldPerTick);
      Debug.Log("Ouro atual: " + gold);
      tickTimer = 0f;
    }
  }

  public void AddGold(int amount)
  {
    if (gold >= maxGold)
    {
      return;
    }
    else
    {
      gold += amount;
    }
    OnGoldChanged?.Invoke(gold);
  }

  public bool SpendGold(int amount)
  {
    if (gold >= amount)
    {
      gold -= amount;
      OnGoldChanged?.Invoke(gold);
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
    Time.timeScale = 0f;
  }

  public void EndGame(bool playerWon)
  {
    jogoAcabou = true;
    if (playerWon)
    {
      Debug.Log("Parabéns! Você venceu!");
      OnGameEnded?.Invoke(true);
      Time.timeScale = 0f;
    }
    else
    {
      Debug.Log("Game Over! Você perdeu!");
      OnGameEnded?.Invoke(false);
      Time.timeScale = 0f;
    }
  }

  internal void InvokeOnRoundChanged(int currentWaveIndex)
  {
    onRoundChanged?.Invoke(currentWaveIndex);
  }
}

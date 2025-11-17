using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
  private static GameStateManager _instance;
  public static GameStateManager Instance { get; private set; }

  [Header("Economia")]
  public int gold = 100;

  [Header("Progresso da Partida")]
  public int actualRound = 1;
  public int VPCount;

  [Header("Condições de Jogo")]
  public bool jogoEmPausa = false;
  public bool jogoAcabou = false;

  private float tickTimer = 0f;

  public event Action<int> OnGoldChanged;

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
    OnGoldChanged?.Invoke(gold);
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
    Time.timeScale = 0f;
  }

  public void EndGame(bool playerWon)
  {
    jogoAcabou = true;
    if (playerWon)
    {
      Debug.Log("Parabéns! Você venceu!");
      Time.timeScale = 0f;
    }
    else
    {
      Debug.Log("Game Over! Você perdeu!");
      Time.timeScale = 0f;
    }
  }

}

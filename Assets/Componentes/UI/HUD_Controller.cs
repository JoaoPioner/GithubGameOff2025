using UnityEngine;
using UnityEngine.UIElements;

public class HUD_Controller : MonoBehaviour
{
  private Label labelGold;
  private VisualElement root;

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;

    labelGold = root.Q<Label>("GoldLabel");
  }

  void Start()
  {
    if (labelGold == null)
    {
      Debug.LogError("GoldLabel não encontrado! Verifique o nome no UI Builder.");
      return;
    }
    GameStateManager.Instance.OnGoldChanged += AtualizarTextoGold;

    AtualizarTextoGold(GameStateManager.Instance.gold);
  }

  void OnDisable()
  {
    if (GameStateManager.Instance != null)
    {
      GameStateManager.Instance.OnGoldChanged -= AtualizarTextoGold;
    }
  }

  private void AtualizarTextoGold(int novoValor)
  {
    labelGold.text = novoValor.ToString();
  }
}

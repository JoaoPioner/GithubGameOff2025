using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD_Controller : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private string goldLabelName = "GoldLabel";
  [SerializeField] private string unitListName = "PieceCards";

  [Header("Card Configuration")]
  public VisualTreeAsset cardTemplate;
  public List<UnitData> unitsAvailable;

  private VisualElement root;
  private Label goldLabel;
  private VisualElement unitsContainer;

  void Awake()
  {
    var uiDocument = GetComponent<UIDocument>();
    root = uiDocument.rootVisualElement;

    goldLabel = root.Q<Label>(goldLabelName);
    unitsContainer = root.Q<VisualElement>(unitListName);
  }

  void Start()
  {
    InitializeGold();
    RenderUnitCards();

    // Faz uma verificação inicial logo que o jogo abre
    UpdateCardsAvailability(GameStateManager.Instance.gold);
  }

  void OnDestroy()
  {
    if (GameStateManager.Instance != null)
    {
      GameStateManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
    }
  }

  // --- LÓGICA DO OURO ---

  void InitializeGold()
  {
    if (GameStateManager.Instance == null) return;

    GameStateManager.Instance.OnGoldChanged += UpdateGoldDisplay;
    UpdateGoldDisplay(GameStateManager.Instance.gold);
  }

  void UpdateGoldDisplay(int newGoldValue)
  {
    if (goldLabel != null)
    {
      goldLabel.text = newGoldValue.ToString();
    }

    // NOVIDADE: Sempre que o ouro mudar, reavaliamos os cards!
    UpdateCardsAvailability(newGoldValue);
  }

  // --- LÓGICA DOS CARDS ---

  void RenderUnitCards()
  {
    if (cardTemplate == null || unitsContainer == null) return;
    unitsContainer.Clear();

    foreach (var unit in unitsAvailable)
    {
      TemplateContainer cardInstance = cardTemplate.Instantiate();

      // --- NOVIDADE: Guardamos os dados DENTRO do elemento visual ---
      // Isso permite que a gente recupere o custo desse card mais tarde sem precisar de uma lista paralela
      cardInstance.userData = unit;

      // Busca elementos (Seus nomes atualizados)
      var priceLabel = cardInstance.Q<Label>("PriceLabel");
      var thumbnailEl = cardInstance.Q<VisualElement>("PieceThumbnail");

      if (priceLabel != null) priceLabel.text = unit.cost.ToString();

      if (thumbnailEl != null && unit.icon != null)
      {
        thumbnailEl.style.backgroundImage = new StyleBackground(unit.icon);
      }

      // Callback de clique
      cardInstance.RegisterCallback<ClickEvent>(evt => OnCardClicked(unit));

      unitsContainer.Add(cardInstance);
    }
  }

  // --- NOVIDADE: A FUNÇÃO QUE DESABILITA ---
  void UpdateCardsAvailability(int currentGold)
  {
    if (unitsContainer == null) return;

    // "Children()" nos dá a lista de todos os cards que já renderizamos na tela
    foreach (var cardVisual in unitsContainer.Children())
    {
      // Recuperamos o UnitData que escondemos no 'userData' lá no RenderUnitCards
      UnitData unitData = cardVisual.userData as UnitData;

      if (unitData != null)
      {
        // A LÓGICA MÁGICA:
        // Se temos dinheiro suficiente, isAffordable é TRUE.
        bool isAffordable = currentGold >= unitData.cost;

        // SetEnabled(false) faz duas coisas:
        // 1. Impede cliques (o botão para de funcionar).
        // 2. Adiciona o pseudo-estado :disabled no CSS (para ficar transparente).
        cardVisual.SetEnabled(isAffordable);
      }
    }
  }

  void OnCardClicked(UnitData unitData)
  {
    if (GameStateManager.Instance.gold < unitData.cost)
    {
      Debug.Log("Sem dinheiro para selecionar!");
      // Opcional: Tocar som de erro ou piscar vermelho
      return;
    }

    Debug.Log($"Selecionou: {unitData.unitName}. Iniciando modo de posicionamento...");

    if (SpawnManager.Instance != null)
    {
      SpawnManager.Instance.SelectPiece(unitData);
    }
    else
    {
      Debug.LogError("SpawnManager não encontrado na cena!");
    }
  }
}
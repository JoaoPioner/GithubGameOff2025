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
  private Label actualRoundLabel;
  private Label maxActualRoundLabel;
  private VisualElement unitsContainer;

  void Awake()
  {
    var uiDocument = GetComponent<UIDocument>();
    root = uiDocument.rootVisualElement;

    goldLabel = root.Q<Label>(goldLabelName);
    actualRoundLabel = root.Q<Label>("ActualRound");
    maxActualRoundLabel = root.Q<Label>("MaxRound");
    unitsContainer = root.Q<VisualElement>(unitListName);
  }

  void Start()
  {
    InitializeGold();
    RenderUnitCards();
    if (GameStateManager.Instance != null)
      maxActualRoundLabel.text = "/ " + GameStateManager.Instance.MaxRounds.ToString();

    UpdateCardsAvailability(GameStateManager.Instance.gold);
  }

  void OnDestroy()
  {
    if (GameStateManager.Instance != null)
    {
      GameStateManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
    }
  }

  void InitializeGold()
  {
    if (GameStateManager.Instance == null) return;

    GameStateManager.Instance.OnGoldChanged += UpdateGoldDisplay;
    GameStateManager.Instance.onRoundChanged += (newRound) =>
    {
      if (actualRoundLabel != null)
      {
        actualRoundLabel.text = newRound.ToString();
      }
    };
    UpdateGoldDisplay(GameStateManager.Instance.gold);
  }

  void UpdateGoldDisplay(int newGoldValue)
  {
    if (goldLabel != null)
    {
      goldLabel.text = newGoldValue.ToString();
    }

    UpdateCardsAvailability(newGoldValue);
  }

  void RenderUnitCards()
  {
    if (cardTemplate == null || unitsContainer == null) return;
    unitsContainer.Clear();

    foreach (var unit in unitsAvailable)
    {
      TemplateContainer cardInstance = cardTemplate.Instantiate();

      cardInstance.userData = unit;

      var priceLabel = cardInstance.Q<Label>("PriceLabel");
      var thumbnailEl = cardInstance.Q<VisualElement>("PieceThumbnail");

      if (priceLabel != null) priceLabel.text = unit.cost.ToString();

      if (thumbnailEl != null && unit.icon != null)
      {
        thumbnailEl.style.backgroundImage = new StyleBackground(unit.icon);
      }

      cardInstance.RegisterCallback<ClickEvent>(evt => OnCardClicked(unit));

      unitsContainer.Add(cardInstance);
    }
  }

  void UpdateCardsAvailability(int currentGold)
  {
    if (unitsContainer == null) return;

    foreach (var cardVisual in unitsContainer.Children())
    {
      UnitData unitData = cardVisual.userData as UnitData;

      if (unitData != null)
      {
        bool isAffordable = currentGold >= unitData.cost;

        cardVisual.SetEnabled(isAffordable);
      }
    }
  }

  void OnCardClicked(UnitData unitData)
  {
    if (GameStateManager.Instance.gold < unitData.cost)
    {
      Debug.Log("Sem dinheiro para selecionar!");
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
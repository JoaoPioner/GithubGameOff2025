using UnityEngine;
using UnityEngine.UIElements; // Importante!

public class HUD_Manager : MonoBehaviour
{
    public VisualElement root;
    // Variáveis para guardar os elementos da UI
    private Label labelGold;

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

    void OnEnable()
    {
        labelGold = root.Q<Label>("GoldLabel");

        if (labelGold == null)
        {
            Debug.Log("GoldLabel not found in the UI hierarchy. Check the name in the UI Builder.");
            return;
        }

        labelGold.text = GameStateManager.Instance.gold.ToString();
    }

    void Update()
    {
        if (root == null || labelGold == null)
        {
            Debug.Log("UI root or GoldLabel is not initialized. Check the UI setup.");
            return;
        }

        labelGold.text = GameStateManager.Instance.gold.ToString();
    }
}
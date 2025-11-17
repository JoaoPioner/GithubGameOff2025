using UnityEngine;

[CreateAssetMenu(fileName = "NovaUnidade", menuName = "TowerDefense/Unidade")]
public class UnitData : ScriptableObject
{
  [Header("Informações Básicas")]
  public string unitName;
  public Sprite icon;
  public int cost;

  [Header("Gameplay")]
  public GameObject prefab;
}
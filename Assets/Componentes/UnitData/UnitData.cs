using UnityEngine;

[CreateAssetMenu(fileName = "NovaUnidade", menuName = "TowerDefense/Unidade")]
public class UnitData : ScriptableObject
{
  [Header("Informações Básicas")]
  public string unitName;
  public Texture2D icon;
  public int cost;

  [Header("Gameplay")]
  public Piece prefab;
}
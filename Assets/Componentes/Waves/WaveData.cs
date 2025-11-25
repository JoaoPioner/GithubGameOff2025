using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField]
    private List<UnitData> _units;

    [SerializeField]
    private float _minSpawnCooldown = 1;
    [SerializeField]
    private float _maxSpawnCooldown = 3;

    public List<UnitData> Units => _units;
    public float MinSpawnCooldown => _minSpawnCooldown;
    public float MaxSpawnCooldown => _maxSpawnCooldown;
}

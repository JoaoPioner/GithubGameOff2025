using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public event Action OnLastWaveFinished;
    
    [SerializeField]
    private List<Transform> spawnPoints;
    
    [SerializeField]
    private List<WaveData> _waves;

    private readonly List<Piece> _activeUnits = new();

    private WaveData _currentWave;
    private int _currentWaveIndex = 0;

    private int _currentUnitIndex = 0;
    
    private Coroutine _spawnCooldownCoroutine;
    
    protected void Start()
    {
        StartWave(_currentWaveIndex);
    }

    private void StartWave(int index)
    {
        if (index >= _waves.Count)
        {
            return;
        }

        Debug.Log($"<color=cyan>Starting Wave {index}</color>");

        _currentWave = _waves[index];
        _currentUnitIndex = 0;

        SpawnUnit(_currentWave.Units[_currentUnitIndex]);
    }

    private void SpawnUnit(UnitData unitData)
    {
        Vector3 position = GetRandomSpawnPoint();
        Piece unitPiece = Instantiate(unitData.prefab, position, Quaternion.identity, gameObject.transform);
        
        _activeUnits.Add(unitPiece);
        unitPiece.OnDeath += OnPieceDeath;

        _currentUnitIndex++;

        if (_currentUnitIndex >= _currentWave.Units.Count)
        {
            return;
        }

        StartSpawnCooldown();
    }

    private void OnPieceDeath(Piece unit)
    {
        _activeUnits.Remove(unit);

        if (WaveHasFinished())
        {
            _currentWaveIndex++;

            if (_currentWaveIndex == _waves.Count)
            {
                Debug.Log($"<color=red>FinishedLast Wave</color>");

                OnLastWaveFinished?.Invoke();
            }
            else
            {
                StartWave(_currentWaveIndex);
            }
        }
    }

    private void StartSpawnCooldown()
    {
        if (_spawnCooldownCoroutine != null)
        {
            StopCoroutine(_spawnCooldownCoroutine);
        }
        
        _spawnCooldownCoroutine = StartCoroutine(UnitSpawnCooldownRoutine());
    }
    
    private IEnumerator UnitSpawnCooldownRoutine()
    {
        float secondsToWait = Random.Range(_currentWave.MinSpawnCooldown, _currentWave.MaxSpawnCooldown);
        yield return new WaitForSeconds(secondsToWait);
        
        SpawnUnit(_currentWave.Units[_currentUnitIndex]);
    }
    
    private Vector3 GetRandomSpawnPoint()
    {
        if(spawnPoints.Count == 0) return new Vector3();
        
        return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
    }

    private bool WaveHasFinished()
    {
        return _activeUnits.Count == 0 && _currentUnitIndex >= _currentWave.Units.Count;
    }
}

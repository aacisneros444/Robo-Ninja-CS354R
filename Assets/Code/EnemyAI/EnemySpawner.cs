using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    private float _spawnRate;
    [SerializeField] private GameObject _enemyPrefab;
    private bool _isPlaying = true;

    private int _numSpawnedAlive = 0;
    private const int MaxSpawnedAlive = 25;

    void Start() {
        DecideSpawnRate();
        StartCoroutine(SpawnEnemy());
        AttackTriggerCollider.KilledEnemy += OnEnemyKilled;
    }

    void OnDestroy() {
        AttackTriggerCollider.KilledEnemy -= OnEnemyKilled;
    }

    private void OnEnemyKilled() {
        _numSpawnedAlive--;
        if (_numSpawnedAlive < 0) {
            _numSpawnedAlive = 0;
        }
    }

    IEnumerator SpawnEnemy() {
        while (_isPlaying) {
            yield return new WaitForSeconds(_spawnRate);
            if (_numSpawnedAlive < MaxSpawnedAlive) {
                Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
                _numSpawnedAlive++;
            }
        }
    }

    private void DecideSpawnRate() {
        GameDifficulty difficulty = GameManger.Instance.CurrentDifficulty;
        switch (difficulty) {
            case GameDifficulty.Easy:
                _spawnRate = 10f;
                break;
            case GameDifficulty.Medium:
                _spawnRate = 7.5f;
                break;
            case GameDifficulty.Harder:
                _spawnRate = 5f;
                break;
            case GameDifficulty.Hard:
                _spawnRate = 2.5f;
                break;
            case GameDifficulty.VeryHard:
                _spawnRate = 1f;
                break;
            default:
                _spawnRate = 10f;
                break;
        }
    }

}

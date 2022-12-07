using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private float _spawnRate;
    [SerializeField] private GameObject _enemyPrefab;
    private bool _isPlaying = true;

    void Start() {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy() {
        while (_isPlaying) {
            yield return new WaitForSeconds(_spawnRate);
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        }
    }

}

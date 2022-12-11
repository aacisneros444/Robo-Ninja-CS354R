using UnityEngine;
using TMPro;

public class OnPlayerWinUI : MonoBehaviour {
    [SerializeField] private TMP_Text _gameWinText;
    [SerializeField] private GameObject _mainMenuButton;
    [SerializeField] private PlayerCameraController _playerCamera;
    [SerializeField] private EnemySpawner _enemySpawner;

    private void Awake() {
        EnemyKillCountDisplay.DestroyedLevelEnemies += OnPlayerWin;
    }

    private void OnDestroy() {
        EnemyKillCountDisplay.DestroyedLevelEnemies -= OnPlayerWin;
    }

    private void OnPlayerWin() {
        _playerCamera.TakingInput = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameWinText.enabled = true;
        _mainMenuButton.SetActive(true);
        _enemySpawner.StopAllCoroutines();
        _enemySpawner.enabled = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++) {
            Destroy(enemies[i]);
        }
    }
}

using UnityEngine;
using TMPro;

public class OnPlayerLoseUI : MonoBehaviour {
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private GameObject _mainMenuButton;
    [SerializeField] private PlayerCamera _playerCamera;

    private void Awake() {
        Health.PlayerDied += OnPlayerLose;
    }

    private void OnDestroy() {
        Health.PlayerDied -= OnPlayerLose;
    }

    private void OnPlayerLose() {
        _playerCamera.TakingInput = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameOverText.enabled = true;
        _mainMenuButton.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    PlayingLevel,
    PlayingTutorial
}

public enum GameDifficulty {
    Easy,
    Medium,
    Harder,
    Hard,
    VeryHard
}

public class GameManger : MonoBehaviour {

    public static GameManger Instance { get; private set; }
    public GameDifficulty CurrentDifficulty;

    [SerializeField] private string _mainMenuScene;
    [SerializeField] private string _gameScene;
    [SerializeField] private string _tutorialScene;

    private GameState _currentState;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
        _currentState = GameState.MainMenu;
        CurrentDifficulty = GameDifficulty.Easy;
        DontDestroyOnLoad(this);
    }

    public void ChangeGameState(GameState newState) {
        if (_currentState == newState) {
            return;
        }
        if (_currentState == GameState.MainMenu) {
            _currentState = newState;
            if (newState == GameState.PlayingLevel) {
                SceneManager.LoadScene(_gameScene);
            } else if (newState == GameState.PlayingTutorial) {
                SceneManager.LoadScene(_tutorialScene);
            }
            return;
        }
        if (_currentState == GameState.PlayingLevel) {
            _currentState = newState;
            SceneManager.LoadScene(_mainMenuScene);
            return;
        }
        if (_currentState == GameState.PlayingTutorial) {
            _currentState = newState;
            SceneManager.LoadScene(_mainMenuScene);
            return;
        }
    }
}

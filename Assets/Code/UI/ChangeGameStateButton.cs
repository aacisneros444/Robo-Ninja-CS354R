using UnityEngine;

public class ChangeGameStateButton : MonoBehaviour {
    [SerializeField] private GameState _gameState;
    public void Clicked() {
        GameManger.Instance.ChangeGameState(_gameState);
    }
}

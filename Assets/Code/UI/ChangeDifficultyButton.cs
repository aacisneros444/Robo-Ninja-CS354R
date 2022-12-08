using UnityEngine;
using TMPro;

public class ChangeDifficultyButton : MonoBehaviour {
    [SerializeField] private TMP_Text _difficultyText;

    private void Start() {
        UpdateButtonText();
    }

    public void Clicked() {
        int difficulty = (int)GameManger.Instance.CurrentDifficulty;
        difficulty++;
        if (difficulty > (int)GameDifficulty.VeryHard) {
            difficulty = 0;
        }
        GameManger.Instance.CurrentDifficulty = (GameDifficulty)difficulty;
        UpdateButtonText();
    }

    private void UpdateButtonText() {
        _difficultyText.text = "Difficulty: " + GameManger.Instance.CurrentDifficulty.ToString();
    }
}

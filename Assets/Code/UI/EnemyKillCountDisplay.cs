using UnityEngine;
using TMPro;
using System;

public class EnemyKillCountDisplay : MonoBehaviour {
    [SerializeField] private int _numEnemiesToKill;
    private int _numEmemiesKilled;
    [SerializeField] private TMP_Text _numKilledText;

    public static event Action DestroyedLevelEnemies;

    private void Awake() {
        AttackTriggerCollider.KilledEnemy += OnEnemyKilled;
    }

    private void OnDestroy() {
        AttackTriggerCollider.KilledEnemy -= OnEnemyKilled;
    }

    private void OnEnemyKilled() {
        _numEmemiesKilled++;
        if (_numEmemiesKilled >= _numEnemiesToKill) {
            DestroyedLevelEnemies?.Invoke();
        }
        _numKilledText.text = _numEmemiesKilled + "/" + _numEnemiesToKill + " destroyed";
    }
}

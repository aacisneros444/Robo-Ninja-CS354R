using UnityEngine;
using System;

public class Health : MonoBehaviour {
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private PlayerController _controller;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private Animator _playerAnimator;
    private int _currentHealth;

    public static event Action<int, int> DamageDealt;
    public static event Action PlayerDied;

    private void Awake() {
        _currentHealth = _maxHealth;
    }

    public void DealDamage(int damage) {
        _currentHealth -= damage;
        if (_currentHealth < 0) {
            _currentHealth = 0;
        }
        DamageDealt?.Invoke(_currentHealth, _maxHealth);
        if (_currentHealth == 0) {
            _controller.enabled = false;
            _playerModel.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            _playerAnimator.Play("BurstSideRight");
            PlayerDied?.Invoke();
        }
    }
}

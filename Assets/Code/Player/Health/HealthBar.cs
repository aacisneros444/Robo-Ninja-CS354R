using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image _healthBar;
    [SerializeField] private AudioSource _hurtSound;
    private bool _hurtAnimPlaying = false;

    private void Awake() {
        Health.DamageDealt += UpdateHealthBar;
    }

    private void OnDestroy() {
        Health.DamageDealt -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth) {
        float percentHealthLeft = (float)currentHealth / maxHealth;
        _healthBar.fillAmount = percentHealthLeft;
        _hurtSound.Play();
        StartCoroutine(DoHealthBarHurtAnimation());
    }

    private IEnumerator DoHealthBarHurtAnimation() {
        if (!_hurtAnimPlaying) {
            _hurtAnimPlaying = true;
            Color originalColor = _healthBar.color;
            Color red = new Color(212f / 255f, 68f / 255f, 64f / 255f);
            Color currColor = originalColor;
            float time = 0f;
            while (time < 0.25f) {
                currColor = Color.Lerp(originalColor, red, (time / 0.25f));
                _healthBar.color = currColor;
                yield return null;
                time += Time.deltaTime;
            }
            time = 0f;
            while (time < 0.25f) {
                currColor = Color.Lerp(red, originalColor, (time / 0.25f));
                _healthBar.color = currColor;
                yield return null;
                time += Time.deltaTime;
            }
            _healthBar.color = originalColor;
            _hurtAnimPlaying = false;
        }
    }
}

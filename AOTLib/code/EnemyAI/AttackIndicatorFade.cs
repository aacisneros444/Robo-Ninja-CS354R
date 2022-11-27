using UnityEngine;

public class AttackIndicatorFade : MonoBehaviour {

    [SerializeField] private float _pauseTimeBeforeFade;
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _startingAlpha;
    [SerializeField] private Renderer _toFade;
    private float _pauseTimeElapsed;
    private float _fadeTimeElapsed;

    private void Update() {
        _pauseTimeElapsed += Time.deltaTime;
        if (_pauseTimeElapsed < _pauseTimeBeforeFade) {
            return;
        }
        if (_fadeTimeElapsed > _fadeTime) {
            Destroy(gameObject);
            return;
        }
        _fadeTimeElapsed += Time.deltaTime;
        float fadePercent = _fadeTimeElapsed / _fadeTime;
        Color matColor = _toFade.material.color;
        float newAlpha = ((1f - fadePercent) * _startingAlpha) / 255f;
        if (newAlpha < 0f) {
            newAlpha = 0f;
        }
        matColor.a = newAlpha;
        _toFade.material.color = matColor;
    }

}
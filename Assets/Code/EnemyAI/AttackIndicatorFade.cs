using UnityEngine;

public class AttackIndicatorFade : MonoBehaviour {

    [SerializeField] private float _pauseTimeBeforeFade;
    [SerializeField] private float _fadeOutTime;
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _fullAlpha;
    [SerializeField] private Renderer _toFade;
    private float _pauseTimeElapsed;
    private float _fadeInTimeElapsed;
    private float _fadeOutTimeElapsed;

    private void Update() {
        _fadeInTimeElapsed += Time.deltaTime;
        if (_fadeInTimeElapsed < _fadeInTime) {
            Fade(false, _fadeInTimeElapsed, _fadeInTime);
            return;
        }

        _pauseTimeElapsed += Time.deltaTime;
        if (_pauseTimeElapsed < _pauseTimeBeforeFade) {
            return;
        }

        _fadeOutTimeElapsed += Time.deltaTime;
        if (_fadeOutTimeElapsed > _fadeOutTime) {
            Destroy(gameObject);
            return;
        }
        Fade(true, _fadeOutTimeElapsed, _fadeOutTime);
    }

    private void Fade(bool invert, float timeElapsed, float fadeTime) {
        float fadePercent = timeElapsed / fadeTime;
        fadePercent = invert ? 1f - fadePercent : fadePercent;
        float newAlpha = (fadePercent * _fullAlpha) / 255f;
        newAlpha = Mathf.Clamp01(newAlpha);
        Color matColor = _toFade.material.color;
        matColor.a = newAlpha;
        _toFade.material.color = matColor;
    }

}
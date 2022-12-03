using UnityEngine;
using System.Collections.Generic;

public class InputHelper {

    private const float DoublePressTime = 0.4f;
    private readonly KeyCode[] keysToTrack = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    private Dictionary<KeyCode, float> _lastKeyPressTimes;

    public InputHelper() {
        _lastKeyPressTimes = new Dictionary<KeyCode, float>();
        foreach (KeyCode key in keysToTrack) {
            _lastKeyPressTimes[key] = 0f;
        }
    }

    public void TrackDoublePressKeys() {
        foreach (KeyCode key in keysToTrack) {
            if (Input.GetKeyDown(key)) {
                _lastKeyPressTimes[key] = Time.realtimeSinceStartup;
            }
        }
    }

    public bool WasKeyDoublePressed(KeyCode key) {
        return (Time.realtimeSinceStartup > 2f) &&
            ((Time.realtimeSinceStartup - _lastKeyPressTimes[key]) < DoublePressTime);
    }
}
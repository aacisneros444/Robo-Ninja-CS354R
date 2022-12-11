using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class to display a crosshair over enemies when the cursor is
/// close enough to an enemy. Visualization of tether assist system.
/// </summary>
public class TetherAssistRenderer : MonoBehaviour {
    [SerializeField] private PlayerControllerData _controllerData;
    [SerializeField] private Image _crosshair;

    private void Update() {
        CheckCursorNearEnemy();
    }

    /// <summary>
    /// Check if the cursor is near an enemy, and display the tether assist crosshair if so.
    /// If cursor not near an enemy, hide the tether assist crosshair.
    /// </summary>
    private void CheckCursorNearEnemy() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitEnemy = Physics.SphereCast(ray, _controllerData.TetherAssistLatchOnRadius,
                                               out RaycastHit castHit, _controllerData.MaxTetherFireDistance,
                                               _controllerData.EnemyLayerMask.value);
        if (hitEnemy) {
            _crosshair.enabled = true;
            _crosshair.transform.position = Camera.main.WorldToScreenPoint(castHit.collider.transform.position);
        } else {
            _crosshair.enabled = false;
        }
    }
}
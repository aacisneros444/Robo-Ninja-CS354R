using UnityEngine;
using UnityEngine.UI;

public class CrosshairRenderer : MonoBehaviour {
    [SerializeField] private float _castRadius;
    [SerializeField] private Image _crosshair;

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.SphereCast(ray, _castRadius,
            out RaycastHit castHit, 150f, ~LayerMask.GetMask("Player"));
        if (hitSomething && castHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            _crosshair.enabled = true;
            _crosshair.transform.position = Camera.main.WorldToScreenPoint(castHit.collider.transform.position);
        } else {
            _crosshair.enabled = false;
        }
    }
}
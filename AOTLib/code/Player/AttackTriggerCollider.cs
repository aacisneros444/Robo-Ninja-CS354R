using UnityEngine;

public class AttackTriggerCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            GameObject.Destroy(other.gameObject);
        }
    }
}
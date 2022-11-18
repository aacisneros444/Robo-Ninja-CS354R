using UnityEngine;

public class SetVel : MonoBehaviour {

    [SerializeField] private Vector3 _velocity;

    private void Start() {
        GetComponent<Rigidbody>().velocity = _velocity;
    }
}
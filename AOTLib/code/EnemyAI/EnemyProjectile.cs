using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    [SerializeField] private ParticleSystem _explosionEffect;
    [SerializeField] private float _speed;
    [SerializeField] private float _aliveTime;
    private float _aliveTimeElapsed;
    private bool _exploded;
    [HideInInspector] public float ExplosionRadius;
    [HideInInspector] public Vector3 ExplodePosition;


    private void Update() {
        if (_exploded) {
            if (!_explosionEffect.isPlaying) {
                Destroy(gameObject);
            }
            return;
        }

        _aliveTimeElapsed += Time.deltaTime;
        if (_aliveTimeElapsed > _aliveTime) {
            Explode();
            return;
        }
        if (Vector3.Distance(transform.position, ExplodePosition) < 1f) {
            Explode();
            return;
        }

        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, _speed * Time.deltaTime, ~LayerMask.GetMask("Enemy"))) {
            Explode();
        } else {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }

    private void Explode() {
        _exploded = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position,
            ExplosionRadius, LayerMask.GetMask("Player"));
        if (colliders.Length > 0) {
            Debug.Log("Hit player!");
        }

        GetComponent<MeshRenderer>().enabled = false;
        _explosionEffect.gameObject.SetActive(true);
        _explosionEffect.Play();
        foreach (Transform child in _explosionEffect.transform) {
            child.GetComponent<ParticleSystem>().Play();
        }
    }
}
using UnityEngine;

public class AttackTriggerCollider : MonoBehaviour {
    // refactor out
    [SerializeField] private ParticleSystem _enemyExplosionParticles;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            // refactor
            ParticleSystem effect = Instantiate(_enemyExplosionParticles, other.transform.position, Quaternion.identity);
            effect.Play();
            effect.GetComponent<AudioSource>().pitch = Random.Range(1.8f, 3f);
            foreach (Transform child in effect.transform) {
                child.GetComponent<ParticleSystem>().Play();
            }
            GameObject.Destroy(other.gameObject);
        }
    }
}
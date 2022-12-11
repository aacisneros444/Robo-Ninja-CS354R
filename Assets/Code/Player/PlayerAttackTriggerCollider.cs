using UnityEngine;

/// <summary>
/// Trigger used to detect if an enemy is hit.
/// </summary>
public class PlayerAttackTriggerCollider : MonoBehaviour {
    [SerializeField] private PlayerControllerData _controllerData;
    [SerializeField] private AudioSource _attackSoundSource;
    public static event System.Action KilledEnemy;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == _controllerData.EnemyLayer) {
            PlayAttackHitSound();
            CreateAttackParticles(other.transform.position);
            GameObject.Destroy(other.gameObject);
            KilledEnemy?.Invoke();
        }
    }

    private void CreateAttackParticles(Vector3 position) {
        ParticleSystem effect = Instantiate(_controllerData.AttackParticlesPrefab, position, Quaternion.identity);
        effect.Play(true);
    }

    private void PlayAttackHitSound() {
        _attackSoundSource.pitch = Random.Range(1.8f, 3f);
        _attackSoundSource.PlayOneShot(_controllerData.AttackHitSound);
    }
}
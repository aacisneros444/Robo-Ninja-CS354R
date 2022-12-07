using UnityEngine;
using System.Collections;


public class EnemyAttackingState : IState {
    private StateMachine _parentFsm;
    private EnemyControllerData _controllerData;
    private Vector3 _attackPosition;

    public EnemyAttackingState(StateMachine parentFsm, EnemyControllerData controllerData) {
        _parentFsm = parentFsm;
        _controllerData = controllerData;
    }

    public void Enter() {
        GenerateAttackPoint();
        _controllerData.rootTransform.GetComponent<MonoBehaviour>().StartCoroutine(WaitToReadyAttack());
    }

    public void Update() {
        RotateToAttackPosition();
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }

    private void GenerateAttackPoint() {
        float ApproxProjectileTravelTime = 0.5f;
        float projectileTravelTimeFactor = _controllerData.playerRb.velocity.magnitude / 22f;
        if (projectileTravelTimeFactor > 1f) {
            projectileTravelTimeFactor = 1f;
        }
        float projectileTravelTime = ApproxProjectileTravelTime * projectileTravelTimeFactor;
        Vector3 predictedPos = _controllerData.playerTransform.position +
            _controllerData.playerRb.velocity * (_controllerData.attackReadyingTime + projectileTravelTime);
        // refactor, 15f should be player's max speed
        float randomnessMultiplier = _controllerData.playerRb.velocity.magnitude / 15f;
        Vector3 randomOffset = Random.insideUnitSphere * Random.Range(0f, 15f * randomnessMultiplier);
        _attackPosition = predictedPos + randomOffset;

        if (!_controllerData.worldOctree.Root.NodeBounds.Contains(_attackPosition)) {
            _attackPosition = _controllerData.worldOctree.Root.NodeBounds.ClosestPoint(_attackPosition);
        }

        Vector3 toAttackPos = _attackPosition - _controllerData.rootTransform.position;
        Vector3 dir = toAttackPos.normalized;
        if (Physics.Raycast(_controllerData.rootTransform.position, dir, out RaycastHit hit,
                            toAttackPos.magnitude, ~LayerMask.GetMask("Enemy"))) {
            // Something in the way to attack position. Change attack pos to hit point.
            _attackPosition = hit.point;
        }
    }

    private IEnumerator WaitToReadyAttack() {
        GameObject attackIndicator = GameObject.Instantiate(_controllerData.attackIndicatorPrefab,
            _attackPosition, Quaternion.identity);
        attackIndicator.transform.localScale = Vector3.one * _controllerData.explosionRadius * 2f;

        yield return new WaitForSeconds(_controllerData.attackReadyingTime);
        Attack();

        _parentFsm.PopState();
        _parentFsm.PushState(new NoDestinationState(_parentFsm, _controllerData, _controllerData.repathRate));
    }

    private void RotateToAttackPosition() {
        Vector3 dirToAttackPos = (_attackPosition - _controllerData.rootTransform.position).normalized;
        _controllerData.rootTransform.forward = dirToAttackPos;
    }

    private void Attack() {
        GameObject projectileObj = GameObject.Instantiate(_controllerData.projectilePrefab,
            _controllerData.projectileOrigin.position, Quaternion.identity);
        projectileObj.transform.forward = _controllerData.rootTransform.transform.forward;

        EnemyProjectile projectile = projectileObj.GetComponent<EnemyProjectile>();
        projectile.ExplodePosition = _attackPosition;
        projectile.ExplosionRadius = _controllerData.explosionRadius;
    }
}
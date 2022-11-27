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
        Vector3 predictedPos = _controllerData.playerTransform.position +
            _controllerData.playerRb.velocity * Time.fixedDeltaTime;
        _attackPosition = predictedPos + Random.insideUnitSphere * Random.Range(0f, 20f);
        if (!_controllerData.worldOctree.Root.NodeBounds.Contains(_attackPosition)) {
            _attackPosition = _controllerData.worldOctree.Root.NodeBounds.ClosestPoint(_attackPosition);
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
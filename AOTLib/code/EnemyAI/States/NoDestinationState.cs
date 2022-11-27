using UnityEngine;
using System.Collections.Generic;

public class NoDestinationState : IState {
    private StateMachine _parentFsm;
    private EnemyControllerData _controllerData;
    private float _timeSinceLastRepath;

    public NoDestinationState(StateMachine parentFsm, EnemyControllerData controllerData, float timeSinceLastRepath) {
        _parentFsm = parentFsm;
        _controllerData = controllerData;
        _timeSinceLastRepath = timeSinceLastRepath;
    }

    public void Enter() {
        _timeSinceLastRepath += Random.Range(0f, _controllerData.repathRate / 2f);
    }

    public void Update() {
        _timeSinceLastRepath += Time.deltaTime;
        if (_timeSinceLastRepath > _controllerData.repathRate) {
            List<Vector3> path = null;
            while (path == null) {
                Vector3 destination = GetDestinationCloseToPlayer();
                path = _controllerData.pathFinder.GetPath(_controllerData.rootTransform.position,
                    destination);
            }
            // Make a beeline if end of path in sight.
            if (EnemyUtils.HasLineOfSightToPosition(_controllerData, path[path.Count - 1])) {
                List<Vector3> newPath = new List<Vector3>();
                newPath.Add(_controllerData.rootTransform.position);
                newPath.Add(path[path.Count - 1]);
                path = newPath;
            }
            _parentFsm.PopState();
            _parentFsm.PushState(new FollowingPathState(_parentFsm, _controllerData, path, 0f));
        } else {
            float distanceToPlayer = Vector3.Distance(_controllerData.rootTransform.position,
                 _controllerData.playerTransform.position);
            if (distanceToPlayer < _controllerData.attackRange) {
                _parentFsm.PopState();
                _parentFsm.PushState(new EnemyAttackingState(_parentFsm, _controllerData));
            }
        }
    }

    private Vector3 GetDestinationCloseToPlayer() {
        Vector3 destinationOffset = Random.insideUnitSphere *
                   _controllerData.destinationOffsetFromPlayer;
        return _controllerData.playerTransform.position + destinationOffset;
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
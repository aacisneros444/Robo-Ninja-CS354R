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

    }

    public void Update() {
        _timeSinceLastRepath += Time.deltaTime;
        if (_timeSinceLastRepath > _controllerData.repathRate) {
            List<Vector3> path = null;
            while (path == null) {
                Vector3 destinationOffset = Random.insideUnitSphere *
                    _controllerData.destinationOffsetFromPlayer;
                path = _controllerData.pathFinder.GetPath(
                    _controllerData.rootTransform.position,
                    _controllerData.playerTransform.position + destinationOffset);
            }
            _parentFsm.PopState();
            _parentFsm.PushState(new FollowingPathState(_parentFsm, _controllerData, path, 0f));
        }
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
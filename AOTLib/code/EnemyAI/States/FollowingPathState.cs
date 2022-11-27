using UnityEngine;
using System.Collections.Generic;

public class FollowingPathState : IState {
    private StateMachine _parentFsm;
    private EnemyControllerData _controllerData;
    private List<Vector3> _path;
    private int _pathIndex;
    private float _timeSinceLastRepath;
    private Vector3 _playerPositionOnStateEnter;

    public FollowingPathState(StateMachine parentFsm, EnemyControllerData controllerData,
        List<Vector3> path, float timeSinceLastRepath) {
        _parentFsm = parentFsm;
        _controllerData = controllerData;
        _path = path;
        _timeSinceLastRepath = timeSinceLastRepath;
        _pathIndex = 1;
    }

    public void Enter() {
        _playerPositionOnStateEnter = _controllerData.playerTransform.position;
        //TryTakeStraightPathToDestination();
        // Debug.Log(_pathIndex + "/" + _path.Count);
    }

    public void Update() {
        _timeSinceLastRepath += Time.deltaTime;

        // Check for path completion.
        if (_pathIndex == _path.Count) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NoDestinationState(_parentFsm, _controllerData, _timeSinceLastRepath));
            return;
        }

        // Check to repath if too long since last repath.
        if (_timeSinceLastRepath > _controllerData.repathRate) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NoDestinationState(_parentFsm, _controllerData, _timeSinceLastRepath));
            return;
        }


        // Repath if player has moved too much.
        if (Vector3.Distance(_playerPositionOnStateEnter, _controllerData.playerTransform.position) > 50f) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NoDestinationState(_parentFsm, _controllerData, _controllerData.repathRate));
            return;
        }

        Vector3 toWaypoint =
            _path[_pathIndex] - _controllerData.rootTransform.position;

        Vector3 dirToWaypoint = toWaypoint.normalized;
        _controllerData.rootTransform.rotation =
            Quaternion.LookRotation(dirToWaypoint, Vector3.up);

        if (toWaypoint.magnitude < 0.5f) {
            _pathIndex++;
            // Debug.Log(_pathIndex + "/" + _path.Count);
            // If there is a clear straight path to the destination at the next waypoint, take it.
            TryTakeStraightPathToDestination();
        }
    }

    public void FixedUpdate() {
        if (_pathIndex == _path.Count) {
            return;
        }
        Vector3 dirToWaypoint =
            (_path[_pathIndex] - _controllerData.rootTransform.position).normalized;
        PhysicsUtils.ChangeVelocityWithMaxAcceleration(_controllerData.rb,
            dirToWaypoint, _controllerData.maxSpeed, _controllerData.maxAcceleration);
    }

    public void Exit() {
        _controllerData.rb.velocity = Vector3.zero;
    }

    private void TryTakeStraightPathToDestination() {
        if (_pathIndex != _path.Count &&
            EnemyUtils.HasLineOfSightToPosition(_controllerData, _path[_path.Count - 1])) {
            List<Vector3> newPath = new List<Vector3>();
            newPath.Add(_controllerData.rootTransform.position);
            newPath.Add(_path[_path.Count - 1]);
            _parentFsm.PopState();
            _parentFsm.PushState(new FollowingPathState(_parentFsm, _controllerData, newPath, _timeSinceLastRepath));
            //Debug.Log("Making a beeline");
        }
    }
}
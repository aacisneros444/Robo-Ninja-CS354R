using UnityEngine;
using System;

public class IsTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    private Vector3 _tetherPoint;
    private float _tetherLength;
    public static event Action<PlayerControllerData, Vector3> EnteredTetherState;

    public IsTetheredState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        Vector3 tetherPoint) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherPoint = tetherPoint;
    }

    public void Enter() {
        _tetherLength = Vector3.Distance(_controllerData.rightTetherOrigin.position, _tetherPoint);
        EnteredTetherState?.Invoke(_controllerData, _tetherPoint);
    }

    public void Update() {
        if (!Input.GetMouseButton(1)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controllerData));
        }
        if (Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsReelingState(_parentFsm,
                _controllerData, _tetherPoint, _tetherLength));
        }
        _controllerData.playerModel.transform.forward =
            (_tetherPoint - _controllerData.rootTransform.position).normalized;
    }

    public void FixedUpdate() {
        TetherUtils.ApplyTetherSpring(_controllerData, _tetherPoint, _tetherLength);
    }

    public void Exit() {

    }
}
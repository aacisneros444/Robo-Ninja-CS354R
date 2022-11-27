using UnityEngine;
using System;

public class IsTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    private Transform _tetherEnd;
    private float _tetherLength;
    public static event Action<PlayerControllerData, Transform> EnteredTetherState;

    public IsTetheredState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        Transform tetherEnd) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherEnd = tetherEnd;
    }

    public void Enter() {
        _tetherLength = Vector3.Distance(_controllerData.rootTransform.position, _tetherEnd.position);
        EnteredTetherState?.Invoke(_controllerData, _tetherEnd);
    }

    public void Update() {
        if (!Input.GetMouseButton(1) || _tetherEnd == null) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controllerData, _tetherEnd));
            return;
        }
        if (Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsReelingState(_parentFsm,
                _controllerData, _tetherEnd, _tetherLength));
        }

        _controllerData.playerModel.transform.forward =
            (_tetherEnd.position - _controllerData.rootTransform.position).normalized;

        TetherUtils.UpdateTetherLength(_controllerData, _tetherEnd.position, ref _tetherLength);
    }

    public void FixedUpdate() {
        if (_tetherEnd == null) {
            return;
        }
        TetherUtils.ApplyTetherSpring(_controllerData, _tetherEnd.position, _tetherLength);
    }

    public void Exit() {

    }
}
using UnityEngine;
using System;

public class IsReelingState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    private Transform _tetherEnd;
    private float _tetherLength;
    public static event Action OnReeling;
    public static event Action OnStopReeling;

    public IsReelingState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        Transform tetherEnd, float tetherLength) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherEnd = tetherEnd;
        _tetherLength = tetherLength;
    }

    public void Enter() {
        OnReeling?.Invoke();
        _controllerData.animator.Play("Reeling");
    }

    public void Update() {
        if (!Input.GetMouseButton(1) || _tetherEnd == null) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controllerData, _tetherEnd));
            return;
        }
        if (!Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsTetheredState(_parentFsm, _controllerData, _tetherEnd));
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
        TetherUtils.ApplyTetherReel(_controllerData, _tetherEnd.position);
    }

    public void Exit() {
        OnStopReeling?.Invoke();
        if (!_controllerData.animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSwingRoll")) {
            _controllerData.animator.Play("Falling");
        }
    }
}
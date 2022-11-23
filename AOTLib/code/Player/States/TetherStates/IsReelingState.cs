using UnityEngine;
using System;

public class IsReelingState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    private Vector3 _tetherPoint;
    private float _tetherLength;
    public static event Action OnReeling;
    public static event Action OnStopReeling;

    public IsReelingState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        Vector3 tetherPoint, float tetherLength) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherPoint = tetherPoint;
        _tetherLength = tetherLength;
    }

    public void Enter() {
        OnReeling?.Invoke();
        _controllerData.animator.Play("Reeling");
    }

    public void Update() {
        if (!Input.GetMouseButton(1)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controllerData));
        }
        if (!Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsTetheredState(_parentFsm, _controllerData, _tetherPoint));
        }
        _controllerData.playerModel.transform.forward =
            (_tetherPoint - _controllerData.rootTransform.position).normalized;
    }

    public void FixedUpdate() {
        TetherUtils.ApplyTetherSpring(_controllerData, _tetherPoint, _tetherLength);
        TetherUtils.ApplyTetherReel(_controllerData, _tetherPoint, ref _tetherLength);
    }

    public void Exit() {
        OnStopReeling?.Invoke();
        _controllerData.animator.Play("Falling");
    }
}
using UnityEngine;
using System;

public class IsReelingState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    private Transform _tetherEnd;
    private float _tetherLength;
    public static event Action OnReeling;
    public static event Action OnStopReeling;

    public IsReelingState(StateMachine parentFsm, PlayerController controller, Transform tetherEnd,
                          float tetherLength) {
        _parentFsm = parentFsm;
        _controller = controller;
        _tetherEnd = tetherEnd;
        _tetherLength = tetherLength;
    }

    public void Enter() {
        OnReeling?.Invoke();
        _controller.PlayerAnimator.Play("Reeling");
    }

    public void Update() {
        if (CheckToUntether()) {
            return;
        }
        if (CheckToStopReeling()) {
            return;
        }

        TetherUtils.UpdateTetherLength(_controller, _tetherEnd.position, ref _tetherLength);
        RotatePlayerModelToTetherEnd();
    }

    public void FixedUpdate() {
        if (_tetherEnd == null) {
            return;
        }
        TetherUtils.ApplyTetherSpring(_controller, _tetherEnd.position, _tetherLength);
        TetherUtils.ApplyTetherReel(_controller, _tetherEnd.position);
    }

    public void Exit() {
        OnStopReeling?.Invoke();
        if (!(_controller.MainFsm.GetCurrentState().GetType() != typeof(PlayerAttackingState))) {
            _controller.PlayerAnimator.Play("Falling");
        }
    }

    private bool CheckToUntether() {
        if (!Input.GetMouseButton(1) || _tetherEnd == null) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controller, _tetherEnd));
            return true;
        }
        return false;
    }

    private bool CheckToStopReeling() {
        if (!Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsTetheredState(_parentFsm, _controller, _tetherEnd));
            return true;
        }
        return false;
    }

    private void RotatePlayerModelToTetherEnd() {
        Vector3 dirToTetherEnd = (_tetherEnd.position - _controller.transform.position).normalized;
        _controller.PlayerModel.transform.forward = dirToTetherEnd;
    }
}
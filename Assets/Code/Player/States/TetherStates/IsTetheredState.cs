using UnityEngine;
using System;

public class IsTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    private Transform _tetherEnd;
    private float _tetherLength;
    public static event Action EnteredState;

    public IsTetheredState(StateMachine parentFsm, PlayerController controller,
        Transform tetherEnd) {
        _parentFsm = parentFsm;
        _controller = controller;
        _tetherEnd = tetherEnd;
    }

    public void Enter() {
        _tetherLength = Vector3.Distance(_controller.transform.position, _tetherEnd.position);
        EnteredState?.Invoke();
    }

    public void Update() {
        if (CheckToUntether()) {
            return;
        }
        if (CheckToReel()) {
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
    }

    public void Exit() {

    }

    private bool CheckToUntether() {
        if (!Input.GetMouseButton(1) || _tetherEnd == null) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotTetheredState(_parentFsm, _controller, _tetherEnd));
            return true;
        }
        return false;
    }

    private bool CheckToReel() {
        if (Input.GetKey(KeyCode.Space)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new IsReelingState(_parentFsm, _controller, _tetherEnd, _tetherLength));
            return true;
        }
        return false;
    }

    private void RotatePlayerModelToTetherEnd() {
        Vector3 dirToTetherEnd = (_tetherEnd.position - _controller.transform.position).normalized;
        _controller.PlayerModel.transform.forward = dirToTetherEnd;
    }
}
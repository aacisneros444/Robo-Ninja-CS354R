using UnityEngine;
using System;

public class NotTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    private Transform _tetherEnd;

    public NotTetheredState(StateMachine parentFsm, PlayerController controller, Transform tetherEnd) {
        _parentFsm = parentFsm;
        _controller = controller;
        _tetherEnd = tetherEnd;
    }

    public void Enter() {
        if (_tetherEnd != null) {
            GameObject.Destroy(_tetherEnd.gameObject);
            _controller.TetherRenderer.DetachTether();
        }
    }

    public void Update() {
        CheckToTether();
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }

    private void CheckToTether() {
        if (Input.GetMouseButtonDown(1)) {
            TryTetherResult tryTetherResult = TetherUtils.TryTether(_controller);
            if (tryTetherResult.tethered) {
                _controller.TetherRenderer.SetTether(tryTetherResult.tetherEnd);
                _parentFsm.PopState();
                _parentFsm.PushState(new IsTetheredState(_parentFsm, _controller, tryTetherResult.tetherEnd));
            }
        }
    }
}
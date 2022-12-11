using UnityEngine;
using System;

public class BurstLeftState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public BurstLeftState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        EnteredState?.Invoke();
        _controller.ThrusterRenderer.AddThrust();
        _controller.PlayerAnimator.Play("BurstSideLeft");
        BurstUtils.DampVelocityForBurst(_controller, -1f * _controller.Cam.transform.right);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.A)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
        }
        _controller.PlayerModel.transform.right = _controller.Cam.transform.right;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, -1f * _controller.Cam.transform.right);
    }

    public void Exit() {
        _controller.ThrusterRenderer.RemoveThrust();
    }
}
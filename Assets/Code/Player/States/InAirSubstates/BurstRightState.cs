using UnityEngine;
using System;

public class BurstRightState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public BurstRightState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        EnteredState?.Invoke();
        _controller.ThrusterRenderer.AddThrust();
        _controller.PlayerAnimator.Play("BurstSideRight");
        BurstUtils.DampVelocityForBurst(_controller, _controller.Cam.transform.right);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.D)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
        }

        _controller.PlayerModel.transform.right = _controller.Cam.transform.right;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, _controller.Cam.transform.right);
    }

    public void Exit() {
        _controller.ThrusterRenderer.RemoveThrust();
    }
}
using UnityEngine;
using System;

public class BurstDownState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public BurstDownState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        EnteredState?.Invoke();
        _controller.ThrusterRenderer.AddThrust();
        _controller.PlayerAnimator.Play("BurstDown");
        BurstUtils.DampVelocityForBurst(_controller, Vector3.down);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.Q)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
        }
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, Vector3.down);
    }

    public void Exit() {
        _controller.ThrusterRenderer.RemoveThrust();
    }
}
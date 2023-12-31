using UnityEngine;
using System;

public class BurstUpState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public BurstUpState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        EnteredState?.Invoke();
        _controller.ThrusterRenderer.AddThrust();
        _controller.PlayerAnimator.Play("BurstUp");
        BurstUtils.DampVelocityForBurst(_controller, Vector3.up);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.E)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
        }
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, Vector3.up);
    }

    public void Exit() {
        _controller.ThrusterRenderer.RemoveThrust();
    }
}
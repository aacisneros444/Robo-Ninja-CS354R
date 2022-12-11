using UnityEngine;
using System;

public class BurstForwardState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public BurstForwardState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        EnteredState?.Invoke();
        _controller.ThrusterRenderer.AddThrust();
        _controller.PlayerAnimator.Play("BurstForward");
        BurstUtils.DampVelocityForBurst(_controller, _controller.Cam.transform.forward);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.W)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
            return;
        }

        _controller.PlayerModel.transform.forward = _controller.Cam.transform.forward;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, _controller.Cam.transform.forward);
    }

    public void Exit() {
        Vector3 originalEuler = _controller.PlayerModel.eulerAngles;
        _controller.PlayerModel.rotation = Quaternion.Euler(0f, originalEuler.y, originalEuler.z);

        _controller.ThrusterRenderer.RemoveThrust();
    }
}
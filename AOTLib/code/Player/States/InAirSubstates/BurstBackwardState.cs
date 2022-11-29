using UnityEngine;
using System;

public class BurstBackwardState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstBackwardState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controller.PlayerAnimator.Play("BurstForward");
        BurstUtils.DampVelocityForBurst(_controller, -1f * _controller.Cam.transform.forward);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.S)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controller));
            return;
        }
        RotatePlayerModelBackwards();
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controller, -_controller.Cam.transform.forward);
    }

    public void Exit() {
        ResetPlayerModelRotation();
        OnExitBurst?.Invoke();
    }

    private void RotatePlayerModelBackwards() {
        _controller.PlayerModel.transform.forward = _controller.Cam.transform.forward;
        Vector3 originalEuler = _controller.PlayerModel.transform.eulerAngles;
        _controller.PlayerModel.transform.rotation = Quaternion.Euler(originalEuler.x + -180f,
                                                                      originalEuler.y, originalEuler.z);
    }

    private void ResetPlayerModelRotation() {
        Vector3 originalEuler = _controller.PlayerModel.eulerAngles;
        _controller.PlayerModel.rotation = Quaternion.Euler(180f, originalEuler.y, originalEuler.z);
    }
}
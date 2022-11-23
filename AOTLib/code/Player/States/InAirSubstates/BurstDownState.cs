using UnityEngine;
using System;

public class BurstDownState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstDownState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controllerData.animator.Play("BurstDown");
        BurstUtils.DampVelocityForBurst(_controllerData, Vector3.down);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.Q)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
        }
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, Vector3.down);
    }

    public void Exit() {
        OnExitBurst?.Invoke();
    }
}
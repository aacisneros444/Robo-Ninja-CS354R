using UnityEngine;
using System;

public class BurstUpState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstUpState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controllerData.animator.Play("BurstUp");
        BurstUtils.DampVelocityForBurst(_controllerData, Vector3.up);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.E)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
        }
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, Vector3.up);
    }

    public void Exit() {
        OnExitBurst?.Invoke();
    }
}
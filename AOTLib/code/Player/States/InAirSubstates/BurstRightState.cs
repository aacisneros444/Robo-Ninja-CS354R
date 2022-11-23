using UnityEngine;
using System;

public class BurstRightState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstRightState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controllerData.animator.Play("BurstSideRight");
        BurstUtils.DampVelocityForBurst(_controllerData, _controllerData.cam.transform.right);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.D)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
        }

        _controllerData.playerModel.transform.right = _controllerData.cam.transform.right;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, _controllerData.cam.transform.right);
    }

    public void Exit() {
        OnExitBurst?.Invoke();
    }
}
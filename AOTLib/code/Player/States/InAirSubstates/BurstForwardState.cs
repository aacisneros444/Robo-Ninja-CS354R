using UnityEngine;
using System;

public class BurstForwardState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstForwardState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controllerData.animator.Play("BurstForward");
        BurstUtils.DampVelocityForBurst(_controllerData, _controllerData.cam.transform.forward);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.W)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
            return;
        }

        _controllerData.playerModel.transform.forward = _controllerData.cam.transform.forward;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, _controllerData.cam.transform.forward);
    }

    public void Exit() {
        Vector3 originalEuler = _controllerData.playerModel.eulerAngles;
        _controllerData.playerModel.rotation = Quaternion.Euler(0f, originalEuler.y, originalEuler.z);
        OnExitBurst?.Invoke();
    }
}
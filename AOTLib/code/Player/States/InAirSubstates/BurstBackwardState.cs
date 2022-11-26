using UnityEngine;
using System;

public class BurstBackwardState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action OnBurst;
    public static event Action OnExitBurst;

    public BurstBackwardState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        OnBurst?.Invoke();
        _controllerData.animator.Play("BurstForward");
        BurstUtils.DampVelocityForBurst(_controllerData, -_controllerData.cam.transform.forward);
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.S)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
            return;
        }

        _controllerData.playerModel.transform.forward = _controllerData.cam.transform.forward;
        Vector3 oldEuler = _controllerData.playerModel.transform.eulerAngles;
        _controllerData.playerModel.transform.rotation = Quaternion.Euler(oldEuler.x + -180f, oldEuler.y, oldEuler.z);
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, -_controllerData.cam.transform.forward);
    }

    public void Exit() {
        Vector3 originalEuler = _controllerData.playerModel.eulerAngles;
        _controllerData.playerModel.rotation = Quaternion.Euler(180f, originalEuler.y, originalEuler.z);
        OnExitBurst?.Invoke();
    }
}
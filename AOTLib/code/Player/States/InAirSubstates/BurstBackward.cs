using UnityEngine;

public class BurstBackwardState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;

    public BurstBackwardState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        _controllerData.rb.velocity = Vector3.zero;
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.S)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
            return;
        }

        _controllerData.playerModel.transform.forward = _controllerData.cam.transform.forward;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, -_controllerData.cam.transform.forward);
    }

    public void Exit() {
        Vector3 originalEuler = _controllerData.playerModel.eulerAngles;
        _controllerData.playerModel.rotation = Quaternion.Euler(0f, originalEuler.y, originalEuler.z);
    }
}
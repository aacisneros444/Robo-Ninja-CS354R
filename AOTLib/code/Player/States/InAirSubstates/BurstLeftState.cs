using UnityEngine;

public class BurstLeftState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;

    public BurstLeftState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        _controllerData.rb.velocity = Vector3.zero;
    }

    public void Update() {
        if (!Input.GetKey(KeyCode.A)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new NotBurstingState(_parentFsm, _controllerData));
        }

        _controllerData.playerModel.transform.right = _controllerData.cam.transform.right;
    }

    public void FixedUpdate() {
        BurstUtils.BurstInDirection(_controllerData, -_controllerData.cam.transform.right);
    }

    public void Exit() {

    }
}
using UnityEngine;

public class BurstDownState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;

    public BurstDownState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        _controllerData.rb.velocity = Vector3.zero;
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

    }
}
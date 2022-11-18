using UnityEngine;

public class PlayerIdleState : IState {
    private StateMachine _fsm;
    private PlayerControllerData _controllerData;
    private PlayerGroundedState _playerGroundedState;

    public PlayerIdleState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        PlayerGroundedState playerGroundedState) {
        _fsm = parentFsm;
        _controllerData = playerControllerData;
        _playerGroundedState = playerGroundedState;
    }

    public void Enter() {

    }

    public void Update() {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
            0f, Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0) {
            _fsm.PopState();
            _fsm.PushState(new PlayerMovingState(_fsm, _controllerData, _playerGroundedState));
        }
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
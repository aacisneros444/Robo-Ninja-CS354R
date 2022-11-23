using UnityEngine;

public class PlayerMovingState : IState {
    private StateMachine _fsm;
    private PlayerControllerData _controllerData;
    private PlayerGroundedState _playerGroundedState;

    public PlayerMovingState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        PlayerGroundedState playerGroundedState) {
        _fsm = parentFsm;
        _controllerData = playerControllerData;
        _playerGroundedState = playerGroundedState;
    }

    public void Enter() {
        _controllerData.animator.Play("Run");
    }

    public void Update() {

    }

    public void FixedUpdate() {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
            0f, Input.GetAxisRaw("Vertical"));

        if (input == Vector3.zero) {
            // Apply stopping force before transitioning.
            _fsm.PopState();
            _fsm.PushState(new PlayerIdleState(_fsm, _controllerData, _playerGroundedState));
            return;
        }

        Vector3 moveDir = (Quaternion.AngleAxis(
            _controllerData.cam.transform.eulerAngles.y, Vector3.up) * input).normalized;
        _playerGroundedState.MoveDir = moveDir;
        _controllerData.playerModel.rotation = Quaternion.LookRotation(moveDir);
    }

    public void Exit() {
        _playerGroundedState.MoveDir = Vector3.zero;
    }
}
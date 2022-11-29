using UnityEngine;

public class PlayerIdleState : IState {
    private StateMachine _fsm;
    private PlayerController _controller;
    private PlayerGroundedState _playerGroundedState;

    public PlayerIdleState(StateMachine parentFsm, PlayerController controller,
                           PlayerGroundedState playerGroundedState) {
        _fsm = parentFsm;
        _controller = controller;
        _playerGroundedState = playerGroundedState;
    }

    public void Enter() {
        _playerGroundedState.MoveDir = Vector3.zero;
        _controller.PlayerAnimator.Play("Idle");
    }

    public void Update() {
        Vector3 input = PlayerControllerUtils.GetRawWASDInput();
        if (input.magnitude > 0) {
            _fsm.PopState();
            _fsm.PushState(new PlayerMovingState(_fsm, _controller, _playerGroundedState));
        }
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
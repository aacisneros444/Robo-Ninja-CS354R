using UnityEngine;
using System;

public class PlayerMovingState : IState {
    private StateMachine _fsm;
    private PlayerController _controller;
    private PlayerGroundedState _playerGroundedState;
    public static event Action EnteredState;

    public PlayerMovingState(StateMachine parentFsm, PlayerController controller,
                             PlayerGroundedState playerGroundedState) {
        _fsm = parentFsm;
        _controller = controller;
        _playerGroundedState = playerGroundedState;
    }

    public void Enter() {
        _controller.PlayerAnimator.Play("Run");
        EnteredState?.Invoke();
    }

    public void Update() {

    }

    public void FixedUpdate() {
        Vector3 input = PlayerControllerUtils.GetRawWASDInput();
        if (input == Vector3.zero) {
            // Apply stopping force before transitioning.
            _fsm.PopState();
            _fsm.PushState(new PlayerIdleState(_fsm, _controller, _playerGroundedState));
            return;
        }

        Vector3 moveDir = GetMoveDirection(input);
        // Set the desired move direction in the parent grounded state class, which will
        // apply the movement.
        _playerGroundedState.MoveDir = moveDir;
        _controller.PlayerModel.rotation = Quaternion.LookRotation(moveDir);
    }

    public void Exit() {

    }

    // Get the direction to move in based on the given WASD input and camera angle.
    private Vector3 GetMoveDirection(Vector3 input) {
        return (Quaternion.AngleAxis(_controller.Cam.transform.eulerAngles.y,
                                     Vector3.up) * input).normalized;
    }
}
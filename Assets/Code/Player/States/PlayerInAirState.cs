using UnityEngine;
using System;

public class PlayerInAirState : IState {
    private StateMachine _parentFsm;
    private StateMachine _burstingSubstatesFsm;
    private PlayerController _controller;
    public static event Action EnteredState;

    public PlayerInAirState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
        _burstingSubstatesFsm = new StateMachine();
        _burstingSubstatesFsm.PushState(new NotBurstingState(_burstingSubstatesFsm, _controller));
    }

    public void Enter() {
        _controller.PlayerAnimator.Play("Falling");
        EnteredState?.Invoke();
    }

    public void Update() {
        ChangeStatesIfNotInAir();
        _burstingSubstatesFsm.Update();
        CheckToAttack();
    }

    public void FixedUpdate() {
        _burstingSubstatesFsm.FixedUpdate();
    }

    public void Exit() {
        _burstingSubstatesFsm.GetCurrentState().Exit();
    }

    private void ChangeStatesIfNotInAir() {
        RaycastHit rayHit = PlayerControllerUtils.PlayerGroundedCheck(_controller);
        bool isGrounded = rayHit.collider != null;
        if (isGrounded) {
            // No longer in air.
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerGroundedState(_parentFsm, _controller));
        }
    }

    private void CheckToAttack() {
        if (Input.GetMouseButtonDown(0)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerAttackingState(_parentFsm, _controller));
        }
    }
}
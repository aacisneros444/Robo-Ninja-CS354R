using UnityEngine;
using System.Collections.Generic;

public class PlayerInAirState : IState {
    private StateMachine _parentFsm;
    private StateMachine _burstingSubstatesFsm;
    private PlayerControllerData _controllerData;

    public PlayerInAirState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _burstingSubstatesFsm = new StateMachine();
        _burstingSubstatesFsm.PushState(new NotBurstingState(_burstingSubstatesFsm, _controllerData));
    }

    public void Enter() {

    }

    public void Update() {
        _burstingSubstatesFsm.GetCurrentState().Update();
        if (Physics.Raycast(_controllerData.rootTransform.position, Vector3.down,
            out RaycastHit rayHit, _controllerData.groundCheckRayLength)) {
            if (rayHit.distance < _controllerData.floatHeight + 0.15f) {
                // No longer in air.
                _parentFsm.PopState();
                _parentFsm.PushState(new PlayerGroundedState(_parentFsm, _controllerData));
            }
        }
    }

    public void FixedUpdate() {
        _burstingSubstatesFsm.GetCurrentState().FixedUpdate();
    }

    public void Exit() {

    }
}
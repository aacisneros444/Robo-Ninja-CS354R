using UnityEngine;

public class PlayerInAirState : IState {
    private StateMachine _parentFsm;
    private StateMachine _burstingSubstatesFsm;
    private StateMachine _tetherFsm;
    private PlayerControllerData _controllerData;

    public PlayerInAirState(StateMachine parentFsm, StateMachine tetherFsm,
        PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _tetherFsm = tetherFsm;
        _controllerData = playerControllerData;
        _burstingSubstatesFsm = new StateMachine();
        _burstingSubstatesFsm.PushState(new NotBurstingState(_burstingSubstatesFsm, _controllerData));
    }

    public void Enter() {
        _controllerData.animator.Play("Falling");
    }

    public void Update() {
        _burstingSubstatesFsm.GetCurrentState().Update();
        _tetherFsm.GetCurrentState().Update();
        const float errorThreshold = 0.15f;
        if (Physics.Raycast(_controllerData.rootTransform.position, Vector3.down,
            out RaycastHit rayHit, _controllerData.groundCheckRayLength,
            ~LayerMask.GetMask("Player"))) {
            if (rayHit.distance < _controllerData.floatHeight + errorThreshold) {
                // No longer in air.
                _parentFsm.PopState();
                _parentFsm.PushState(new PlayerGroundedState(_parentFsm,
                    _tetherFsm, _controllerData));
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            // _controllerData.playerModel.rotation = Quaternion.LookRotation(_controllerData.cam.transform.forward);
            // _controllerData.animator.Play("SwordSwingRoll");
            // PhysicsUtils.ChangeVelocityWithMaxAcceleration(_controllerData.rb, _controllerData.cam.transform.forward,
            //     _controllerData.maxBurstSpeed, 5000f);
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerAttackingState(_parentFsm, _tetherFsm, _controllerData));
        }
    }

    public void FixedUpdate() {
        _burstingSubstatesFsm.GetCurrentState().FixedUpdate();
        _tetherFsm.GetCurrentState().FixedUpdate();
    }

    public void Exit() {
        _burstingSubstatesFsm.GetCurrentState().Exit();
    }
}
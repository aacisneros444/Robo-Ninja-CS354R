using UnityEngine;

public class PlayerGroundedState : IState {
    private StateMachine _parentFsm;
    private StateMachine _groundedSubstatesFsm;
    private StateMachine _tetherFsm;
    private PlayerControllerData _controllerData;
    public Vector3 MoveDir;
    private bool _jumped;

    public PlayerGroundedState(StateMachine parentFsm, StateMachine tetherFsm,
        PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _groundedSubstatesFsm = new StateMachine();
        _groundedSubstatesFsm.PushState(new PlayerIdleState(_groundedSubstatesFsm, _controllerData, this));
        _tetherFsm = tetherFsm;
    }

    public void Enter() {
        Vector3 originalEuler = _controllerData.playerModel.transform.eulerAngles;
        _controllerData.playerModel.transform.rotation = Quaternion.Euler(0f, originalEuler.y, originalEuler.z);
    }

    public void Update() {
        _groundedSubstatesFsm.GetCurrentState().Update();
        _tetherFsm.GetCurrentState().Update();
        const float errorThreshold = 0.15f;
        if (!Physics.Raycast(_controllerData.rootTransform.position, Vector3.down,
            _controllerData.floatHeight + errorThreshold, ~LayerMask.GetMask("Player"))) {
            // No longer grounded.
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerInAirState(_parentFsm,
                _tetherFsm, _controllerData));
        }
        if (Input.GetKeyDown(KeyCode.E) && !_jumped) {
            _jumped = true;
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
        _groundedSubstatesFsm.GetCurrentState().FixedUpdate();
        _tetherFsm.GetCurrentState().FixedUpdate();
        if (_jumped) {
            _jumped = false;
            ApplyJumpForce();
        }
        ApplyFloatForce();
        if (_tetherFsm.GetCurrentState().GetType() != typeof(IsReelingState)) {
            ApplyMovement(MoveDir);
        }
    }

    public void Exit() {

    }

    private void ApplyFloatForce() {
        if (Physics.Raycast(_controllerData.rootTransform.position, Vector3.down,
            out RaycastHit rayHit, _controllerData.groundCheckRayLength)) {
            if (rayHit.distance < _controllerData.floatHeight) {
                Vector3 hitRbVelocity = Vector3.zero;
                if (rayHit.rigidbody != null) {
                    hitRbVelocity = rayHit.rigidbody.velocity;
                }

                // Find the amount of velocity in the direction of the raycast
                // for our own rigidbody and a rigidbody we may have hit.
                float downVelocity = Vector3.Dot(Vector3.down, _controllerData.rb.velocity);
                float otherRbVelocity = Vector3.Dot(Vector3.down, hitRbVelocity);

                float relativeVelocity = downVelocity - otherRbVelocity;

                float x = rayHit.distance - _controllerData.floatHeight;
                // spring formula: f = -kx-bv
                // where f is force to apply, k is spring stiffness, x is displacement,
                // b is dampening value, and v is velocity
                float springForce = -1 * ((_controllerData.floatSpringStrength * x) -
                    (_controllerData.floatSpringDamper * relativeVelocity));
                _controllerData.rb.AddForce(Vector3.up * springForce);
            }
        }
    }

    private void ApplyMovement(Vector3 direction) {
        // Determine if character is on a rigidbody to
        // save that body's velocity. It will need to
        // be applied to the character (think 
        // moving platform).
        RaycastHit rayHit;
        Vector3 groundVelocity = Vector3.zero;
        if (Physics.Raycast(_controllerData.rootTransform.position,
            Vector3.down, out rayHit, _controllerData.groundCheckRayLength)) {
            if (rayHit.rigidbody != null) {
                groundVelocity = rayHit.rigidbody.velocity;
            }
        } else {
            // Can't move while not grounded.
            return;
        }
        Vector3 goalVelocity = (direction.normalized * _controllerData.maxSpeed) + groundVelocity;
        Vector3 oldRbVelocity = new Vector3(_controllerData.rb.velocity.x, 0f,
            _controllerData.rb.velocity.z);
        Vector3 goalAccel = (goalVelocity - oldRbVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, _controllerData.maxAcceleration);
        // F=ma
        _controllerData.rb.AddForce(_controllerData.rb.mass * goalAccel);
    }

    private void ApplyJumpForce() {
        _controllerData.rb.AddForce(Vector3.up * _controllerData.jumpStrength);
    }
}
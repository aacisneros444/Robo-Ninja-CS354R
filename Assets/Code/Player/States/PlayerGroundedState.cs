using UnityEngine;

public class PlayerGroundedState : IState {
    private StateMachine _parentFsm;
    private StateMachine _groundedSubstatesFsm;
    private PlayerController _controller;
    // The current direction to move the player.
    public Vector3 MoveDir;
    private bool _jumped;

    public PlayerGroundedState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
        _groundedSubstatesFsm = new StateMachine();
        _groundedSubstatesFsm.PushState(new PlayerIdleState(_groundedSubstatesFsm, _controller, this));
    }

    public void Enter() {
        Vector3 originalEuler = _controller.PlayerModel.transform.eulerAngles;
        _controller.PlayerModel.transform.rotation = Quaternion.Euler(0f, originalEuler.y, originalEuler.z);
    }

    public void Update() {
        ChangeStatesIfNotGrounded();
        _groundedSubstatesFsm.Update();
        CheckToJump();
        CheckToAttack();
    }

    public void FixedUpdate() {
        _groundedSubstatesFsm.FixedUpdate();
        ApplyFloatForce();
        if (_jumped) {
            _jumped = false;
            ApplyJumpForce();
        }
        if (_controller.TetherFsm.GetCurrentState().GetType() != typeof(IsReelingState)) {
            ApplyMovement();
        }
    }

    public void Exit() {

    }

    private void ChangeStatesIfNotGrounded() {
        RaycastHit rayHit = PlayerControllerUtils.PlayerGroundedCheck(_controller);
        bool isGrounded = rayHit.collider != null;
        if (!isGrounded) {
            // No longer grounded.
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerInAirState(_parentFsm, _controller));
        }
    }

    private void CheckToJump() {
        if (Input.GetKeyDown(KeyCode.E) && !_jumped) {
            _jumped = true;
        }
    }

    private void CheckToAttack() {
        if (Input.GetMouseButtonDown(0)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new PlayerAttackingState(_parentFsm, _controller));
        }
    }

    private void ApplyFloatForce() {
        RaycastHit rayHit = PlayerControllerUtils.HitGroundCheck(_controller);
        if (rayHit.collider == null) {
            // Did not hit ground, shouldn't apply float force.
            return;
        }

        if (rayHit.distance < _controller.ControllerData.FloatHeight) {
            Vector3 hitRbVelocity = Vector3.zero;
            if (rayHit.rigidbody != null) {
                hitRbVelocity = rayHit.rigidbody.velocity;
            }

            // Find the amount of velocity in the direction of the raycast
            // for our own rigidbody and a rigidbody we may have hit.
            float downVelocity = Vector3.Dot(Vector3.down, _controller.Rb.velocity);
            float otherRbVelocity = Vector3.Dot(Vector3.down, hitRbVelocity);
            float relativeVelocity = downVelocity - otherRbVelocity;

            float x = rayHit.distance - _controller.ControllerData.FloatHeight;
            PhysicsUtils.ApplySpringForce(_controller.Rb, Vector3.up,
                                          _controller.ControllerData.FloatSpringStrength,
                                          x, _controller.ControllerData.FloatSpringDamper,
                                          relativeVelocity, true, false);
        }
    }

    private void ApplyMovement() {
        // Determine if character is on a rigidbody to
        // save that body's velocity. It will need to
        // be applied to the character (think 
        // moving platform).
        RaycastHit rayHit = PlayerControllerUtils.HitGroundCheck(_controller);
        Vector3 groundVelocity = Vector3.zero;
        bool hitGround = rayHit.collider != null;
        if (hitGround && rayHit.rigidbody != null) {
            groundVelocity = rayHit.rigidbody.velocity;
        }

        Vector3 currVelocity = new Vector3(_controller.Rb.velocity.x, 0f, _controller.Rb.velocity.z);
        Vector3 goalVelocity = (MoveDir * _controller.ControllerData.GroundedMaxSpeed) + groundVelocity;
        PhysicsUtils.ChangeVelocityWithMaxAcceleration(_controller.Rb, currVelocity, goalVelocity,
                                                       _controller.ControllerData.GroundedMaxAcceleration);
    }

    private void ApplyJumpForce() {
        _controller.Rb.AddForce(Vector3.up * _controller.ControllerData.JumpStrength);
    }
}
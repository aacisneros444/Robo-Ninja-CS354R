using UnityEngine;
using System.Collections;

public class PlayerAttackingState : IState {
    private StateMachine _parentFsm;
    private StateMachine _groundedSubstatesFsm;
    private PlayerController _controller;
    public Vector3 MoveDir;
    private bool _jumped;

    public PlayerAttackingState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
    }

    public void Enter() {
        _controller.PlayerAnimator.Play("SwordSwingRoll");
        _controller.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(WaitForAnimFinish());
        _controller.AttackTriggerCollider.SetActive(true);
    }

    public void Update() {

    }

    public void FixedUpdate() {

    }

    public void Exit() {
        _controller.AttackTriggerCollider.SetActive(false);
    }

    private IEnumerator WaitForAnimFinish() {
        const float AttackAnimTime = 0.635f;
        yield return new WaitForSeconds(AttackAnimTime);
        _parentFsm.PopState();
        RaycastHit rayHit = PlayerControllerUtils.PlayerGroundedCheck(_controller);
        bool grounded = rayHit.collider != null;
        if (grounded) {
            _parentFsm.PushState(new PlayerGroundedState(_parentFsm, _controller));
        } else {
            _parentFsm.PushState(new PlayerInAirState(_parentFsm, _controller));
        }
    }
}
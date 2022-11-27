using UnityEngine;
using System.Collections;

public class PlayerAttackingState : IState {
    private StateMachine _parentFsm;
    private StateMachine _groundedSubstatesFsm;
    private StateMachine _tetherFsm;
    private PlayerControllerData _controllerData;
    public Vector3 MoveDir;
    private bool _jumped;

    public PlayerAttackingState(StateMachine parentFsm, StateMachine tetherFsm,
        PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherFsm = tetherFsm;
    }

    public void Enter() {
        _controllerData.animator.Play("SwordSwingRoll");
        _controllerData.rootTransform.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(WaitForAnimFinish());
        _controllerData.attackTriggerCollider.SetActive(true);
    }

    public void Update() {
        _tetherFsm.GetCurrentState().Update();
    }

    public void FixedUpdate() {
        _tetherFsm.GetCurrentState().FixedUpdate();
    }

    public void Exit() {
        _controllerData.attackTriggerCollider.SetActive(false);
    }

    private IEnumerator WaitForAnimFinish() {
        const float animTime = 0.635f;
        yield return new WaitForSeconds(animTime);
        _parentFsm.PopState();
        const float errorThreshold = 0.15f;
        bool grounded = !Physics.Raycast(_controllerData.rootTransform.position, Vector3.down,
            _controllerData.floatHeight + errorThreshold, ~LayerMask.GetMask("Player"));
        if (grounded) {
            _parentFsm.PushState(new PlayerGroundedState(_parentFsm, _tetherFsm, _controllerData));
        } else {
            _parentFsm.PushState(new PlayerInAirState(_parentFsm, _tetherFsm, _controllerData));
        }
    }
}
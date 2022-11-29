using UnityEngine;

public class NotBurstingState : IState {

    private StateMachine _parentFsm;
    private PlayerController _controller;
    private InputHelper _inputHelper;

    public NotBurstingState(StateMachine parentFsm, PlayerController controller) {
        _parentFsm = parentFsm;
        _controller = controller;
        _inputHelper = new InputHelper();
    }

    public void Enter() {
        _controller.PlayerAnimator.Play("Falling");
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstUpState(_parentFsm, _controller));
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstDownState(_parentFsm, _controller));
        } else if (Input.GetKeyDown(KeyCode.W) && _inputHelper.WasKeyDoublePressed(KeyCode.W)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstForwardState(_parentFsm, _controller));
        } else if (Input.GetKeyDown(KeyCode.A) && _inputHelper.WasKeyDoublePressed(KeyCode.A)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstLeftState(_parentFsm, _controller));
        } else if (Input.GetKeyDown(KeyCode.S) && _inputHelper.WasKeyDoublePressed(KeyCode.S)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstBackwardState(_parentFsm, _controller));
        } else if (Input.GetKeyDown(KeyCode.D) && _inputHelper.WasKeyDoublePressed(KeyCode.D)) {
            _parentFsm.PopState();
            _parentFsm.PushState(new BurstRightState(_parentFsm, _controller));
        }
        _inputHelper.TrackDoublePressKeys();
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
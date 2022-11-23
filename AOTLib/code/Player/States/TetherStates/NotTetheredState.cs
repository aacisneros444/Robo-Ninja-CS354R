using UnityEngine;
using System;

public class NotTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    public static event Action ExitedTetherState;

    public NotTetheredState(StateMachine parentFsm, PlayerControllerData playerControllerData) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
    }

    public void Enter() {
        ExitedTetherState?.Invoke();
    }

    public void Update() {
        if (Input.GetMouseButtonDown(1)) {
            TryTetherResult tryTetherResult = TetherUtils.TryTether(_controllerData);
            if (tryTetherResult.tethered) {
                _parentFsm.PopState();
                _parentFsm.PushState(new IsTetheredState(_parentFsm,
                    _controllerData, tryTetherResult.tetherPoint));
            }
        }
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
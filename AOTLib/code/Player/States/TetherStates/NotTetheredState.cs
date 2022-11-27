using UnityEngine;
using System;

public class NotTetheredState : IState {

    private StateMachine _parentFsm;
    private PlayerControllerData _controllerData;
    private Transform _tetherEnd;
    public static event Action ExitedTetherState;

    public NotTetheredState(StateMachine parentFsm, PlayerControllerData playerControllerData,
        Transform tetherEnd) {
        _parentFsm = parentFsm;
        _controllerData = playerControllerData;
        _tetherEnd = tetherEnd;
    }

    public void Enter() {
        if (_tetherEnd != null) {
            GameObject.Destroy(_tetherEnd.gameObject);
        }
        ExitedTetherState?.Invoke();
    }

    public void Update() {
        if (Input.GetMouseButtonDown(1)) {
            TryTetherResult tryTetherResult = TetherUtils.TryTether(_controllerData);
            if (tryTetherResult.tethered) {
                _parentFsm.PopState();
                _parentFsm.PushState(new IsTetheredState(_parentFsm,
                    _controllerData, tryTetherResult.tetherEnd));
            }
        }
    }

    public void FixedUpdate() {

    }

    public void Exit() {

    }
}
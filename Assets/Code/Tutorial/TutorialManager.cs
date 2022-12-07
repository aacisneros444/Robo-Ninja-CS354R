using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private GameObject _welcomeText;
    [SerializeField] private GameObject _groundMoveInstructionsText;
    [SerializeField] private GameObject _wellDoneText;
    [SerializeField] private GameObject _jumpingInstructionsText;
    [SerializeField] private GameObject _tetherInstructionsText;
    [SerializeField] private GameObject _reelInstructionsText;
    [SerializeField] private GameObject _adSwingInstructionsText;
    [SerializeField] private GameObject _swingTipText;
    [SerializeField] private GameObject _forwardBurstInstructionsText;
    [SerializeField] private GameObject _backwardBurstInstructionsText;
    [SerializeField] private GameObject _leftBurstInstructionsText;
    [SerializeField] private GameObject _rightBurstInstructionsText;
    [SerializeField] private GameObject _upBurstInstructionsText;
    [SerializeField] private GameObject _downBurstInstructionsText;
    [SerializeField] private GameObject _learnToFightText;
    [SerializeField] private GameObject _killDummiesInstruction;
    [SerializeField] private TMP_Text _enemiesKilledText;
    [SerializeField] private GameObject _dummiesParent;
    [SerializeField] private GameObject _crosshairTipText;
    [SerializeField] private GameObject _killEnemiesInstructionText;
    [SerializeField] private GameObject _enemiesParent;
    [SerializeField] private GameObject _positiveFightingFeedbackText;
    [SerializeField] private GameObject _endTutorialText;


    private bool _didWelcome;
    private bool _playerMovedGrounded, _displayedGroundedMovementTutorial;
    private bool _canTutorialAdvance;
    private bool _playerJumped, _displayedJumpTutorial;
    private bool _playerTethered, _displayedTetheredTutorial;
    private bool _playerReeled, _displayedReeledTutorial;
    private bool _playerADSwung, _displayedADSwingTutorial;
    private bool _playerForwardBurst, _displayedForwardBurstTutorial;
    private bool _playerBackwardBurst, _displayedBackwardBurstTutorial;
    private bool _playerLeftBurst, _displayedLeftBurstTutorial;
    private bool _playerRightBurst, _displayedRightBurstTutorial;
    private bool _playerUpBurst, _displayedUpBurstTutorial;
    private bool _playerDownBurst, _displayedDownBurstTutorial;
    private bool _playerKilledDummies, _displayedDummyTutorial;
    private int _numDummiesKilled;
    private bool _playerKilledEnemies, _displayedEnemiesTutorial;
    private int _numEnemiesKilled;

    private void Start() {
        StartCoroutine(DisplayWelcome());
    }

    private void OnDestroy() {
        PlayerMovingState.EnteredState -= OnPlayerMovedGrounded;
        PlayerInAirState.EnteredState -= OnPlayerJumped;
        IsTetheredState.EnteredTetherState -= OnPlayerTethered;
        IsReelingState.OnReeling -= OnPlayerReeled;
        TetherUtils.PlayerSwungAround -= OnPlayerSwungAroundObject;
        BurstForwardState.OnBurst -= OnPlayerForwardBurst;
        BurstBackwardState.OnBurst -= OnPlayerBackwardBurst;
        BurstLeftState.OnBurst -= OnPlayerLeftBurst;
        BurstRightState.OnBurst -= OnPlayerRightBurst;
        BurstUpState.OnBurst -= OnPlayerUpBurst;
        BurstDownState.OnBurst -= OnPlayerDownBurst;
        AttackTriggerCollider.KilledEnemy -= OnPlayerKilledDummy;
        AttackTriggerCollider.KilledEnemy -= OnPlayerKilledEnemy;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameManger.Instance.ChangeGameState(GameState.MainMenu);
        }
        if (!_didWelcome) {
            return;
        }
        if (!_playerMovedGrounded && _canTutorialAdvance) {
            if (!_displayedGroundedMovementTutorial) {
                DisplayGroundedMovementTutorial();
            }
            return;
        }
        if (!_playerJumped && _canTutorialAdvance) {
            if (!_displayedJumpTutorial) {
                DisplayJumpTutorial();
            }
            return;
        }
        if (!_playerTethered && _canTutorialAdvance) {
            if (!_displayedTetheredTutorial) {
                DisplayTetherTutorial();
            }
            return;
        }
        if (!_playerReeled && _canTutorialAdvance) {
            if (!_displayedReeledTutorial) {
                DisplayReelTutorial();
            }
            return;
        }
        if (!_playerADSwung && _canTutorialAdvance) {
            if (!_displayedADSwingTutorial) {
                DisplayLeftRightSwingTutorial();
            }
            return;
        }
        if (!_playerForwardBurst && _canTutorialAdvance) {
            if (!_displayedForwardBurstTutorial) {
                DisplayForwardBurstTutorial();
            }
            return;
        }
        if (!_playerBackwardBurst && _canTutorialAdvance) {
            if (!_displayedBackwardBurstTutorial) {
                DisplayBackwardsBurstTutorial();
            }
            return;
        }
        if (!_playerLeftBurst && _canTutorialAdvance) {
            if (!_displayedLeftBurstTutorial) {
                DisplayLeftBurstTutorial();
            }
            return;
        }
        if (!_playerRightBurst && _canTutorialAdvance) {
            if (!_displayedRightBurstTutorial) {
                DisplayRightBurstTutorial();
            }
            return;
        }
        if (!_playerUpBurst && _canTutorialAdvance) {
            if (!_displayedUpBurstTutorial) {
                DisplayUpBurstTutorial();
            }
            return;
        }
        if (!_playerDownBurst && _canTutorialAdvance) {
            if (!_displayedDownBurstTutorial) {
                DisplayDownBurstTutorial();
            }
            return;
        }
        if (!_playerKilledDummies && _canTutorialAdvance) {
            if (!_displayedDummyTutorial) {
                DisplayDummyTutorial();
            }
            return;
        }
        if (!_playerKilledEnemies && _canTutorialAdvance) {
            if (!_displayedEnemiesTutorial) {
                DisplayEnemiesTutorial();
            }
            return;
        }
    }

    private IEnumerator DisplayWelcome() {
        _welcomeText.SetActive(true);
        yield return new WaitForSeconds(2f);
        _welcomeText.SetActive(false);
        _didWelcome = true;
        _canTutorialAdvance = true;
    }

    private void DisplayGroundedMovementTutorial() {
        _displayedGroundedMovementTutorial = true;
        _groundMoveInstructionsText.SetActive(true);
        PlayerMovingState.EnteredState += OnPlayerMovedGrounded;
    }

    private void OnPlayerMovedGrounded() {
        PlayerMovingState.EnteredState -= OnPlayerMovedGrounded;
        _groundMoveInstructionsText.SetActive(false);
        _playerMovedGrounded = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private IEnumerator DisplayPositiveFeedback() {
        _canTutorialAdvance = false;
        _wellDoneText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _wellDoneText.SetActive(false);
        _canTutorialAdvance = true;
    }

    private void DisplayJumpTutorial() {
        _displayedJumpTutorial = true;
        _jumpingInstructionsText.SetActive(true);
        PlayerInAirState.EnteredState += OnPlayerJumped;
    }

    private void OnPlayerJumped() {
        PlayerInAirState.EnteredState -= OnPlayerJumped;
        _jumpingInstructionsText.SetActive(false);
        _playerJumped = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayTetherTutorial() {
        _displayedTetheredTutorial = true;
        _tetherInstructionsText.SetActive(true);
        IsTetheredState.EnteredTetherState += OnPlayerTethered;
    }

    private void OnPlayerTethered(Transform t) {
        IsTetheredState.EnteredTetherState -= OnPlayerTethered;
        _tetherInstructionsText.SetActive(false);
        _playerTethered = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayReelTutorial() {
        _displayedReeledTutorial = true;
        _reelInstructionsText.SetActive(true);
        IsReelingState.OnReeling += OnPlayerReeled;
    }

    private void OnPlayerReeled() {
        IsReelingState.OnReeling -= OnPlayerReeled;
        _reelInstructionsText.SetActive(false);
        _playerReeled = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayLeftRightSwingTutorial() {
        _displayedADSwingTutorial = true;
        _adSwingInstructionsText.SetActive(true);
        TetherUtils.PlayerSwungAround += OnPlayerSwungAroundObject;
    }

    private void OnPlayerSwungAroundObject() {
        TetherUtils.PlayerSwungAround -= OnPlayerSwungAroundObject;
        _adSwingInstructionsText.SetActive(false);
        _playerADSwung = true;
        StartCoroutine(DisplaySwingTip());
    }

    private IEnumerator DisplaySwingTip() {
        _canTutorialAdvance = false;
        _wellDoneText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _wellDoneText.SetActive(false);
        _swingTipText.SetActive(true);
        yield return new WaitForSeconds(6f);
        _swingTipText.SetActive(false);
        _canTutorialAdvance = true;
    }

    private void DisplayForwardBurstTutorial() {
        _displayedForwardBurstTutorial = true;
        _forwardBurstInstructionsText.SetActive(true);
        BurstForwardState.OnBurst += OnPlayerForwardBurst;
    }

    private void OnPlayerForwardBurst() {
        BurstForwardState.OnBurst -= OnPlayerForwardBurst;
        _forwardBurstInstructionsText.SetActive(false);
        _playerForwardBurst = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayBackwardsBurstTutorial() {
        _displayedBackwardBurstTutorial = true;
        _backwardBurstInstructionsText.SetActive(true);
        BurstBackwardState.OnBurst += OnPlayerBackwardBurst;
    }

    private void OnPlayerBackwardBurst() {
        BurstBackwardState.OnBurst -= OnPlayerBackwardBurst;
        _backwardBurstInstructionsText.SetActive(false);
        _playerBackwardBurst = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayLeftBurstTutorial() {
        _displayedLeftBurstTutorial = true;
        _leftBurstInstructionsText.SetActive(true);
        BurstLeftState.OnBurst += OnPlayerLeftBurst;
    }

    private void OnPlayerLeftBurst() {
        BurstLeftState.OnBurst -= OnPlayerLeftBurst;
        _leftBurstInstructionsText.SetActive(false);
        _playerLeftBurst = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayRightBurstTutorial() {
        _displayedRightBurstTutorial = true;
        _rightBurstInstructionsText.SetActive(true);
        BurstRightState.OnBurst += OnPlayerRightBurst;
    }

    private void OnPlayerRightBurst() {
        BurstRightState.OnBurst -= OnPlayerRightBurst;
        _rightBurstInstructionsText.SetActive(false);
        _playerRightBurst = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayUpBurstTutorial() {
        _displayedUpBurstTutorial = true;
        _upBurstInstructionsText.SetActive(true);
        BurstUpState.OnBurst += OnPlayerUpBurst;
    }

    private void OnPlayerUpBurst() {
        BurstUpState.OnBurst -= OnPlayerUpBurst;
        _upBurstInstructionsText.SetActive(false);
        _playerUpBurst = true;
        StartCoroutine(DisplayPositiveFeedback());
    }

    private void DisplayDownBurstTutorial() {
        _displayedDownBurstTutorial = true;
        _downBurstInstructionsText.SetActive(true);
        BurstDownState.OnBurst += OnPlayerDownBurst;
    }

    private void OnPlayerDownBurst() {
        BurstDownState.OnBurst -= OnPlayerDownBurst;
        _downBurstInstructionsText.SetActive(false);
        _playerDownBurst = true;
        StartCoroutine(DisplayLearnToFightText());
    }

    private IEnumerator DisplayLearnToFightText() {
        _canTutorialAdvance = false;
        _learnToFightText.SetActive(true);
        yield return new WaitForSeconds(3f);
        _learnToFightText.SetActive(false);
        _canTutorialAdvance = true;
    }


    private void DisplayDummyTutorial() {
        _displayedDummyTutorial = true;
        _killDummiesInstruction.SetActive(true);
        _enemiesKilledText.gameObject.SetActive(true);
        _dummiesParent.SetActive(true);
        AttackTriggerCollider.KilledEnemy += OnPlayerKilledDummy;
    }

    private void OnPlayerKilledDummy() {
        _numDummiesKilled++;
        const int NumDummiesToKill = 5;
        _enemiesKilledText.text = _numDummiesKilled + "/" + NumDummiesToKill + " destroyed";
        if (_numDummiesKilled < NumDummiesToKill) {
            return;
        }
        AttackTriggerCollider.KilledEnemy -= OnPlayerKilledDummy;
        _playerKilledDummies = true;
        _killDummiesInstruction.SetActive(false);
        _enemiesKilledText.gameObject.SetActive(false);
        _dummiesParent.SetActive(false);
        StartCoroutine(DisplayCrosshairTip());
    }

    private IEnumerator DisplayCrosshairTip() {
        _canTutorialAdvance = false;
        _wellDoneText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _wellDoneText.SetActive(false);
        _crosshairTipText.SetActive(true);
        yield return new WaitForSeconds(15f);
        _crosshairTipText.SetActive(false);
        _canTutorialAdvance = true;
    }

    private void DisplayEnemiesTutorial() {
        _displayedEnemiesTutorial = true;
        _killEnemiesInstructionText.SetActive(true);
        _enemiesKilledText.text = "0/5 destroyed";
        _enemiesKilledText.gameObject.SetActive(true);
        StartCoroutine(SpawnEnemies());
        AttackTriggerCollider.KilledEnemy += OnPlayerKilledEnemy;
    }

    private void OnPlayerKilledEnemy() {
        _numEnemiesKilled++;
        const int NumEnemiesToKill = 5;
        _enemiesKilledText.text = _numEnemiesKilled + "/" + NumEnemiesToKill + " destroyed";
        if (_numEnemiesKilled < NumEnemiesToKill) {
            return;
        }
        AttackTriggerCollider.KilledEnemy -= OnPlayerKilledEnemy;
        _playerKilledEnemies = true;
        _killEnemiesInstructionText.SetActive(false);
        _enemiesKilledText.gameObject.SetActive(false);
        _enemiesParent.SetActive(false);
        StartCoroutine(DisplayEndTutorial());
    }

    IEnumerator SpawnEnemies() {
        yield return new WaitForSeconds(8f);
        _enemiesParent.SetActive(true);
    }

    private IEnumerator DisplayEndTutorial() {
        _positiveFightingFeedbackText.SetActive(true);
        yield return new WaitForSeconds(5f);
        _positiveFightingFeedbackText.SetActive(false);
        _endTutorialText.SetActive(true);
    }

}

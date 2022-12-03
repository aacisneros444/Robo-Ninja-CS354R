using UnityEngine;

[CreateAssetMenu]
public class PlayerControllerData : ScriptableObject {
    [Header("Float/Hover Settings")]
    [SerializeField] private float _groundCheckRayLength = 1f;
    [SerializeField] private float _floatHeight = 0.5f;
    [SerializeField] private float _floatSpringStrength = 4000f;
    [SerializeField] private float _floatSpringDamper = 250f;

    [Header("Grounded Locomotion Settings")]
    [SerializeField] private float _groundedMaxSpeed = 7f;
    [SerializeField] private float _groundedMaxAcceleration = 150f;
    [SerializeField] private float _jumpStrength = 2700f;

    [Header("Burst Settings")]
    [SerializeField] private float _burstStartDampFactor = 0.5f;
    [SerializeField] private float _maxBurstSpeed = 22f;
    [SerializeField] private float _maxBurstAcceleration = 75f;

    [Header("Tether Settings")]
    [SerializeField] private float _maxTetherFireDistance = 150f;
    [SerializeField] private float _tetherSpringStrength = 250f;
    [SerializeField] private float _tetherSpringDamper = 200f;
    [SerializeField] private float _maxReelSpeed = 28f;
    [SerializeField] private float _maxReelAcceleration = 55f;
    [SerializeField] private float _horitontalSwingStrength = 17f;

    [Header("Attacking Settings")]
    [SerializeField] private float _aimAssistLatchOnRadius = 2.5f;


    // Read-Only Properties for above fields.

    // Float/Hover Settings
    public float GroundCheckRayLength => _groundCheckRayLength;
    public float FloatHeight => _floatHeight;
    public float FloatSpringStrength => _floatSpringStrength;
    public float FloatSpringDamper => _floatSpringDamper;

    // Grounded Locomotion Settings
    public float GroundedMaxSpeed => _groundedMaxSpeed;
    public float GroundedMaxAcceleration => _groundedMaxAcceleration;
    public float JumpStrength => _jumpStrength;

    // Burst Settings
    public float BurstStartDampFactor => _burstStartDampFactor;
    public float MaxBurstSpeed => _maxBurstSpeed;
    public float MaxBurstAcceleration => _maxBurstAcceleration;

    // Tether Settings
    public float MaxTetherFireDistance => _maxTetherFireDistance;
    public float TetherSpringStrength => _tetherSpringStrength;
    public float TetherSpringDamper => _tetherSpringDamper;
    public float MaxReelSpeed => _maxReelSpeed;
    public float MaxReelAcceleration => _maxReelAcceleration;
    public float HoritontalSwingStrength => _horitontalSwingStrength;

    // Attacking Settings
    public float AimAssistLatchOnRadius => _aimAssistLatchOnRadius;
}
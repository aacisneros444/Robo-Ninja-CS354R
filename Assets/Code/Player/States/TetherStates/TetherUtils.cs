using UnityEngine;
using System;

public struct TryTetherResult {
    public bool tethered;
    public Transform tetherEnd;
}

public static class TetherUtils {
    // for tutorial, refactor..?
    public static event Action PlayerSwungAround;

    // Try to tether using a raycast from camera center.
    public static TryTetherResult TryTether(PlayerController controller) {
        TryTetherResult tetherData = new TryTetherResult {
            tethered = false,
            tetherEnd = null
        };

        // Tether result data
        bool gotHit = false;
        RaycastHit rayHit = new RaycastHit();

        int castLayers = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Level Bounds"));
        // Check if any enemies are in the camera direction with a spherecast.
        Transform enemyTransform = GetEnemyInTetherDirection(controller);
        if (enemyTransform) {
            // Got closest enemy transform, now get closest point on enemy collider
            // to attach to.
            Vector3 dir = (enemyTransform.position - controller.Cam.transform.position).normalized;
            gotHit = Physics.Raycast(controller.Cam.transform.position, dir,
                                     out rayHit, controller.ControllerData.MaxTetherFireDistance,
                                     castLayers);
        }
        if (!gotHit) {
            // Couldn't latch to an enemy in camera facing direction. 
            // Try a simple raycast to tether to a surface.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            gotHit = Physics.Raycast(ray, out rayHit, controller.ControllerData.MaxTetherFireDistance,
                                     castLayers);
        }

        if (gotHit) {
            // Successfully tethered. 
            GameObject tetherEnd = new GameObject();
            tetherEnd.name = "Tether End";
            tetherEnd.transform.position = rayHit.point;
            tetherEnd.transform.parent = rayHit.collider.transform;

            tetherData.tethered = true;
            tetherData.tetherEnd = tetherEnd.transform;
        }
        return tetherData;
    }

    /// <summary>
    /// Check if any enemies lie in the camera facing direction using a sphere
    /// cast.
    /// </summary>
    /// <param name="controller">The player controller</param>
    /// <returns>A transform from the hit enemy if a hit occured, null otherwise.</returns>
    public static Transform GetEnemyInTetherDirection(PlayerController controller) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.SphereCast(ray, controller.ControllerData.AimAssistLatchOnRadius,
                                               out RaycastHit castHit, controller.ControllerData.MaxTetherFireDistance,
                                               LayerMask.GetMask("Enemy"));
        if (hitSomething) {
            return castHit.transform;
        }
        return null;
    }

    // Keep the player tethered within tether length with a spring force.
    public static void ApplyTetherSpring(PlayerController controller,
                                         Vector3 tetherPoint, float tetherLength) {
        float distanceToTetherEnd = Vector3.Distance(tetherPoint, controller.transform.position);
        const float ApplySpringThreshold = 0.5f;
        if (distanceToTetherEnd > tetherLength + ApplySpringThreshold) {
            // Apply spring if player is further away than tether length.
            Vector3 dirToTetherEnd = (tetherPoint - controller.transform.position).normalized;
            float velocityInDir = Vector3.Dot(dirToTetherEnd, controller.Rb.velocity);
            PhysicsUtils.ApplySpringForce(controller.Rb,
                                          dirToTetherEnd,
                                          controller.ControllerData.TetherSpringStrength,
                                          distanceToTetherEnd,
                                          controller.ControllerData.TetherSpringDamper,
                                          velocityInDir,
                                          false, true);
        }
    }

    // Reel in the tether direction.
    public static void ApplyTetherReel(PlayerController controller, Vector3 tetherPoint) {
        Vector3 reelDir = (tetherPoint - controller.transform.position).normalized;

        // Swing a bit to the left or right if A or D is pressed.
        float adInput = Input.GetAxisRaw("Horizontal");
        if (adInput != 0) {
            float distanceToTether = Vector3.Distance(tetherPoint, controller.transform.position);
            float rightLeftStrength = Mathf.Clamp01(1f / distanceToTether);
            if (rightLeftStrength > 0.15f) {
                PlayerSwungAround?.Invoke();
            }
            Vector3 swing = Vector3.Cross(Vector3.up, reelDir) * rightLeftStrength *
                                          controller.ControllerData.HoritontalSwingStrength;
            if (adInput < 0) {
                // Negate if for cross product flipping.
                swing *= -1;
            }
            reelDir += swing;
            reelDir.Normalize();
        }

        Vector3 goalVelocity = reelDir * controller.ControllerData.MaxReelSpeed;
        PhysicsUtils.ChangeVelocityWithMaxAcceleration(controller.Rb, goalVelocity,
                                                       controller.ControllerData.MaxReelAcceleration);
    }

    // Update the tether length for the current state.
    public static void UpdateTetherLength(PlayerController controller, Vector3 tetherPoint,
                                          ref float tetherLength) {
        float distanceToTether = Vector3.Distance(controller.transform.position, tetherPoint);
        if (distanceToTether < tetherLength) {
            tetherLength = distanceToTether;
        }
    }
}
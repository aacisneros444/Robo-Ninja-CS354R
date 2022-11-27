using UnityEngine;

public static class TetherUtils {
    // Try to tether using a raycast from camera center.
    public static TryTetherResult TryTether(PlayerControllerData controllerData) {
        TryTetherResult tetherData = new TryTetherResult {
            tethered = false,
            tetherEnd = null
        };

        bool gotHit = false;
        RaycastHit rayHit = new RaycastHit();
        // Check if any enemies are in the camera direction with a spherecast.
        Transform enemyTransform = GetEnemyInTetherDirection(controllerData);
        if (enemyTransform) {
            // Got closest enemy transform, now get closest point on enemy collider
            // to attach to.
            Vector3 dir = (enemyTransform.position - controllerData.cam.transform.position).normalized;
            gotHit = Physics.Raycast(controllerData.cam.transform.position, dir,
                out rayHit, controllerData.maxTetherFireDistance, ~LayerMask.GetMask("Player"));
        }
        if (!gotHit) {
            // No enemies in camera facing direction. Try a simple raycast to tether to a surface.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            gotHit = Physics.Raycast(ray, out rayHit,
                    controllerData.maxTetherFireDistance, ~LayerMask.GetMask("Player"));
        }

        if (gotHit) {
            GameObject tetherEnd = new GameObject();
            tetherEnd.name = "Tether End";
            tetherEnd.transform.position = rayHit.point;
            tetherEnd.transform.parent = rayHit.collider.transform;

            tetherData.tethered = true;
            tetherData.tetherEnd = tetherEnd.transform;
        }
        return tetherData;
    }

    public static Transform GetEnemyInTetherDirection(PlayerControllerData controllerData) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.SphereCast(ray, controllerData.enemySphereCastLatchRadius,
                  out RaycastHit castHit, controllerData.maxTetherFireDistance, LayerMask.GetMask("Enemy"));
        if (hitSomething) {
            return castHit.transform;
        }
        return null;
    }

    // Keep the player tethered within tether length with a spring force.
    public static void ApplyTetherSpring(PlayerControllerData controllerData,
        Vector3 tetherPoint, float tetherLength) {
        float distance = Vector3.Distance(tetherPoint, controllerData.rootTransform.position);
        const float errorThreshold = 0.5f;
        if (distance > tetherLength + errorThreshold) {
            // Apply spring if player is further away than tether length.
            Vector3 dir = (tetherPoint - controllerData.rootTransform.position).normalized;
            float dirVelocity = Vector3.Dot(dir, controllerData.rb.velocity);
            float springForce = ((controllerData.tetherSpringStrength * distance) -
                (controllerData.tetherSpringDamper * dirVelocity));
            if (springForce > 0f) {
                // Force should only be restorative.
                controllerData.rb.AddForce(dir * springForce);
            }
        }
    }

    // Reel in the tether direction.
    public static void ApplyTetherReel(PlayerControllerData controllerData, Vector3 tetherPoint) {
        Vector3 dir = (tetherPoint - controllerData.rootTransform.position).normalized;

        // Swing a bit to the left or right if A or D is pressed.
        float adInput = Input.GetAxisRaw("Horizontal");
        float distanceToTether = Vector3.Distance(tetherPoint,
            controllerData.rootTransform.position);
        if (adInput != 0) {
            float rightLeftStrength = Mathf.Clamp01(1f / distanceToTether);
            Vector3 swing = Vector3.Cross(Vector3.up, dir) *
                rightLeftStrength * controllerData.horitontalSwingStrength;
            if (adInput < 0) {
                swing *= -1;
            }
            dir += swing;
            dir.Normalize();
        }

        PhysicsUtils.ChangeVelocityWithMaxAcceleration(controllerData.rb, dir,
            controllerData.maxReelSpeed, controllerData.maxReelAcceleration);
    }

    // Update the tether length for the current state.
    public static void UpdateTetherLength(PlayerControllerData controllerData, Vector3 tetherPoint,
        ref float tetherLength) {
        float distanceToTether = Vector3.Distance(controllerData.rootTransform.position, tetherPoint);
        if (distanceToTether < tetherLength) {
            tetherLength = distanceToTether;
        }
    }
}

public struct TryTetherResult {
    public bool tethered;
    public Transform tetherEnd;
}
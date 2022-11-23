using UnityEngine;

public static class TetherUtils {
    // Try to tether using a raycast from camera center.
    public static TryTetherResult TryTether(PlayerControllerData controllerData) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        TryTetherResult tetherData = new TryTetherResult {
            tethered = false,
            tetherPoint = Vector3.zero
        };
        if (Physics.Raycast(ray, out RaycastHit rayHit,
            controllerData.maxTetherFireDistance)) {
            tetherData.tethered = true;
            tetherData.tetherPoint = rayHit.point;
        }
        return tetherData;
    }

    // Keep the player tethered within tether length with a spring force.
    public static void ApplyTetherSpring(PlayerControllerData controllerData,
        Vector3 tetherPoint, float tetherLength) {
        float distance = Vector3.Distance(tetherPoint, controllerData.rightTetherOrigin.position);
        const float errorThreshold = 0.5f;
        if (distance > tetherLength + errorThreshold) {
            Vector3 dir = (tetherPoint - controllerData.rightTetherOrigin.position).normalized;
            float dirVelocity = Vector3.Dot(dir, controllerData.rb.velocity);
            float springForce = ((controllerData.tetherSpringStrength * distance) -
                (controllerData.tetherSpringDamper * dirVelocity));
            controllerData.rb.AddForce(dir * springForce);
        }
    }

    // Reel in the tether direction.
    public static void ApplyTetherReel(PlayerControllerData controllerData,
        Vector3 tetherPoint, ref float tetherLength) {
        Vector3 dir = (tetherPoint - controllerData.rootTransform.position).normalized;
        // Swing a bit to the left or right if A or D is pressed.
        float adInput = Input.GetAxisRaw("Horizontal");
        if (adInput != 0) {
            float distanceToTether = Vector3.Distance(tetherPoint,
                controllerData.rootTransform.position);
            float rightLeftStrength = Mathf.Clamp01(1f / distanceToTether);
            Vector3 swing = Vector3.Cross(Vector3.up, dir) *
                rightLeftStrength * controllerData.horitontalSwingStrength;
            if (adInput < 0) {
                swing *= -1;
            }
            dir += swing;
            dir.Normalize();
        }

        Vector3 goalVelocity = (dir * controllerData.maxReelSpeed);
        Vector3 oldRbVelocity = controllerData.rb.velocity;
        Vector3 goalAccel = (goalVelocity - oldRbVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, controllerData.maxReelAcceleration);
        // F=ma
        controllerData.rb.AddForce(controllerData.rb.mass * goalAccel);

        tetherLength = Vector3.Distance(controllerData.rightTetherOrigin.position, tetherPoint);
    }
}

public struct TryTetherResult {
    public bool tethered;
    public Vector3 tetherPoint;
}
using UnityEngine;

public static class BurstUtils {
    public static void BurstInDirection(PlayerControllerData controllerData, Vector3 direction) {
        Vector3 goalVelocity = (direction * controllerData.maxBurstSpeed);

        Vector3 oldRbVelocity = controllerData.rb.velocity;
        Vector3 goalAccel = (goalVelocity - oldRbVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, controllerData.maxBurstAcceleration);

        // F=ma
        controllerData.rb.AddForce(controllerData.rb.mass * goalAccel);
    }
}
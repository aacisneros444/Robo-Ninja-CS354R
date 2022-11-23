using UnityEngine;

public static class BurstUtils {

    public static void DampVelocityForBurst(PlayerControllerData controllerData,
        Vector3 burstDirection) {
        float velDirdiff = Vector3.Dot(controllerData.rb.velocity.normalized, burstDirection);
        float dampFactor = velDirdiff > 0.5f ? velDirdiff : controllerData.burstStartDampFactor;
        controllerData.rb.velocity =
            controllerData.rb.velocity * dampFactor;
    }

    public static void BurstInDirection(PlayerControllerData controllerData, Vector3 direction) {
        PhysicsUtils.ChangeVelocityWithMaxAcceleration(controllerData.rb, direction,
            controllerData.maxBurstSpeed, controllerData.maxBurstAcceleration);
    }
}
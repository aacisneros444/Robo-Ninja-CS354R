using UnityEngine;

public static class BurstUtils {

    public static void DampVelocityForBurst(PlayerController controller, Vector3 burstDirection) {
        float velDirDiff = Vector3.Dot(controller.Rb.velocity.normalized, burstDirection);
        float dampFactor = velDirDiff > 0.5f ? velDirDiff : controller.ControllerData.BurstStartDampFactor;
        controller.Rb.velocity = controller.Rb.velocity * dampFactor;
    }

    public static void BurstInDirection(PlayerController controller, Vector3 direction) {
        Vector3 goalVelocity = direction * controller.ControllerData.MaxBurstSpeed;
        PhysicsUtils.ChangeVelocityWithMaxAcceleration(controller.Rb, goalVelocity,
                                                       controller.ControllerData.MaxBurstAcceleration);
    }
}
using UnityEngine;

public static class PhysicsUtils {
    /// <summary>
    /// Change a rigidbody's current velocity to reach a target speed
    /// under a max acceleration.
    /// </summary>
    /// <param name="rb">The rigidbody to change velocity for.</param>
    /// <param name="dir">The direction of the velocity.</param>
    /// <param name="maxSpeed">The target velocity magnitude.</param>
    /// <param name="maxAcceleration">The max acceleration allowed in this fixed update.</param>
    public static void ChangeVelocityWithMaxAcceleration(Rigidbody rb, Vector3 dir,
        float maxSpeed, float maxAcceleration) {
        Vector3 goalVelocity = (dir * maxSpeed);
        Vector3 oldRbVelocity = rb.velocity;
        Vector3 goalAccel = (goalVelocity - oldRbVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, maxAcceleration);
        // F=ma
        rb.AddForce(rb.mass * goalAccel);
    }
}
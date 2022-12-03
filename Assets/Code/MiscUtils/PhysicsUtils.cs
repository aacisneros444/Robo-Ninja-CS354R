using UnityEngine;

public static class PhysicsUtils {

    /// <summary>
    /// Change a rigidbody's current velocity to reach a target velocity
    /// under a max acceleration.
    /// </summary>
    /// <param name="rb">The rb to change velocity for.</param>
    /// <param name="goalVelocity">The target velocity.</param>
    /// <param name="maxAcceleration">The maximum the rb can accelerate.</param>
    public static void ChangeVelocityWithMaxAcceleration(Rigidbody rb, Vector3 goalVelocity,
                                                         float maxAcceleration) {
        ChangeVelocityWithMaxAcceleration(rb, rb.velocity, goalVelocity, maxAcceleration);
    }

    /// <summary>
    /// Change a rigidbody's current velocity to reach a target velocity
    /// under a max acceleration.
    /// </summary>
    /// <param name="rb">The rb to change velocity for.</param>
    /// <param name="currentVelocity">The current velocity.</param>
    /// <param name="goalVelocity">The target velocity.</param>
    /// <param name="maxAcceleration">The maximum the rb can accelerate.</param>
    public static void ChangeVelocityWithMaxAcceleration(Rigidbody rb, Vector3 currentVelocity,
                                                         Vector3 goalVelocity, float maxAcceleration) {
        Vector3 goalAccel = (goalVelocity - currentVelocity) / Time.fixedDeltaTime;
        goalAccel = Vector3.ClampMagnitude(goalAccel, maxAcceleration);
        // F=ma
        rb.AddForce(rb.mass * goalAccel);
    }

    /// <summary>
    /// Apply a spring force to a rigidbody based on Hooke's Law.
    /// </summary>
    /// <param name="rb">The rigidbody to apply a force on.</param>
    /// <param name="dir">The direction to apply the force.</param>
    /// <param name="k">The spring's stiffness.</param>
    /// <param name="x">The displacement from the equilibrium position.</param>
    /// <param name="b">A dampening value.</param>
    /// <param name="v">Calculated velocity in the given direction.</param>
    /// <param name="negate">Flips the calculated spring force's sign if true.</param>
    /// <param name="onlyPositive">Only allow positive spring force if true.</param>
    public static void ApplySpringForce(Rigidbody rb, Vector3 dir, float k, float x,
                                        float b, float v, bool negate, bool onlyPositive) {
        float springForce = (k * x) - (b * v);
        if (negate) {
            springForce *= -1f;
        }
        if (onlyPositive && springForce < 0f) {
            return;
        }
        rb.AddForce(dir * springForce);
    }
}
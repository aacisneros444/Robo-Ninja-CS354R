using UnityEngine;

public static class PlayerControllerUtils {

    /// <summary>
    /// Get raw WASD input for this frame.
    /// </summary>
    /// <returns>A Vector3 with input values for this frame. 
    /// The vector's x component will contain the Horizontal axis
    /// input value. The vector's y component will contain the Vertical 
    /// axis input value.</returns>
    public static Vector3 GetRawWASDInput() {
        return new Vector3(Input.GetAxisRaw("Horizontal"),
                           0f, Input.GetAxisRaw("Vertical"));
    }

    /// <summary>
    /// Raycast to see if ground is hit using ground check ray length.
    /// </summary>
    /// <param name="controller">The PlayerController.</param>
    /// <returns>A raycast hit from the check. If hit.collider != null,
    /// then ground was hit. False otherwise.</returns>
    public static RaycastHit HitGroundCheck(PlayerController controller) {
        Physics.Raycast(controller.transform.position,
                        Vector3.down, out RaycastHit rayHit,
                        controller.ControllerData.GroundCheckRayLength);
        return rayHit;
    }

    /// <summary>
    /// Raycast to see if the player is currently grounded or not.
    /// </summary>
    /// <param name="controller">The PlayerController.</param>
    /// <returns>A raycast hit from the check. If hit.collider != null,
    /// then ground was hit. False otherwise.</returns>
    public static RaycastHit PlayerGroundedCheck(PlayerController controller) {
        const float ErrorThreshold = 0.15f;
        Physics.Raycast(controller.transform.position, Vector3.down, out RaycastHit rayHit,
                        controller.ControllerData.FloatHeight + ErrorThreshold,
                        ~LayerMask.GetMask("Player"));
        return rayHit;
    }
}
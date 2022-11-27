using UnityEngine;

public static class EnemyUtils {
    public static bool HasLineOfSightToPosition(EnemyControllerData controllerData,
        Vector3 targetPosition) {
        Vector3 dir = (targetPosition - controllerData.rootTransform.position).normalized;
        const float raycastDistance = 250f;
        return !Physics.Raycast(controllerData.rootTransform.position, dir,
            raycastDistance, ~LayerMask.GetMask("Enemy"));
    }
}
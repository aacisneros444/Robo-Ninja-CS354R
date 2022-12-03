using UnityEngine;

public static class EnemyUtils {
    public static bool HasStraightPathToPosition(EnemyControllerData controllerData,
                                                 Vector3 targetPosition) {
        Vector3 toTarget = targetPosition - controllerData.rootTransform.position;
        Vector3 dir = toTarget.normalized;
        float castDistance = toTarget.magnitude;
        return !Physics.SphereCast(controllerData.rootTransform.position,
            controllerData.agentRadius, dir, out RaycastHit hit,
            castDistance, ~LayerMask.GetMask("Enemy"));
    }
}
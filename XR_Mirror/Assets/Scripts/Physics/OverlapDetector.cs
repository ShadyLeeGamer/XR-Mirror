using UnityEngine;

public class OverlapDetector : ColliderDetector
{
    #region Optimised
    Collider[] colliderThisFrame = new Collider[1];
    protected override Collider[] SphereDetectCollidersOptimised(Vector3 center, float radius, LayerMask mask)
        => Physics.OverlapSphereNonAlloc(center, radius, colliderThisFrame, mask) > 0
        ? FinalTargets(colliderThisFrame)
        : null;

    protected override Collider[] BoxDetectCollidersOptimised(Vector3 center, Vector3 halfExtents, Quaternion rotation, LayerMask mask)
        => Physics.OverlapBoxNonAlloc(center, halfExtents, colliderThisFrame, rotation, mask) > 0
        ? FinalTargets(colliderThisFrame)
        : null;

    protected override Collider[] CapsuleDetectCollidersOptimised(Vector3 point0, Vector3 point1, float radius, LayerMask mask)
        => Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colliderThisFrame, mask) > 0
        ? FinalTargets(colliderThisFrame)
        : null;
    #endregion

    #region Normal
    protected override Collider[] SphereDetectColliders(Vector3 center, float radius, LayerMask mask)
        => Physics.OverlapSphere(center, radius, mask);

    protected override Collider[] BoxDetectColliders(Vector3 center, Vector3 halfExtents, Quaternion rotation, LayerMask mask)
        => Physics.OverlapBox(center, halfExtents, transform.rotation, mask);

    protected override Collider[] CapsuleDetectColliders(Vector3 point0, Vector3 point1, float radius, LayerMask mask)
        => Physics.OverlapCapsule(point0, point1, radius, mask);
    #endregion
}
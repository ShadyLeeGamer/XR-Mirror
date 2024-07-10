using UnityEngine;

public class CastDetector : ColliderDetector
{
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 Direction => velocity.normalized;
    float Distance
    {
        get
        {
            float distance = velocity.magnitude;
            // Fix detection not working with 0 or very low distance
            if (distance <= 0)
            {
                distance = 0.1f;
            }
            return distance;
        }
    }

    protected override void Update()
    {
        base.Update();

        velocity = transform.position - lastPos;
        lastPos = transform.position;
    }

    #region Optimised
    RaycastHit[] hitsThisFrame = new RaycastHit[1];
    protected override Collider[] SphereDetectCollidersOptimised(Vector3 center, float radius, LayerMask mask)
        => Physics.SphereCastNonAlloc(center, radius, Direction, hitsThisFrame, Distance, mask) > 0
        ? FinalTargets(RaycastHitsToColliders(hitsThisFrame))
        : null;

    protected override Collider[] BoxDetectCollidersOptimised(Vector3 center, Vector3 size, Quaternion rotation, LayerMask mask)
        => Physics.BoxCastNonAlloc(center, size, Direction, hitsThisFrame, rotation, Distance, mask) > 0
        ? FinalTargets(RaycastHitsToColliders(hitsThisFrame))
        : null;

    protected override Collider[] CapsuleDetectCollidersOptimised(Vector3 point0, Vector3 point1, float radius, LayerMask mask)
        => Physics.CapsuleCastNonAlloc(point0, point1, radius, Direction, hitsThisFrame, Distance, mask) > 0
        ? FinalTargets(RaycastHitsToColliders(hitsThisFrame))
        : null;
    #endregion

    #region Normal
    protected override Collider[] SphereDetectColliders(Vector3 center, float radius, LayerMask mask)
        => RaycastHitsToColliders(
            Physics.SphereCastAll(center, radius, Direction, Distance, mask)
        );

    protected override Collider[] BoxDetectColliders(Vector3 center, Vector3 halfExtents, Quaternion rotation, LayerMask mask)
        => RaycastHitsToColliders(
            Physics.BoxCastAll(center, halfExtents, Direction, transform.rotation, Distance, mask)
        );

    protected override Collider[] CapsuleDetectColliders(Vector3 point0, Vector3 point1, float radius, LayerMask mask)
        => RaycastHitsToColliders(
            Physics.CapsuleCastAll(point0, point1, radius, Direction, Distance, mask)
        );
    #endregion

    #region Helpers
    Collider[] RaycastHitsToColliders(RaycastHit[] hits)
    {
        Collider[] colliders = new Collider[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            colliders[i] = hits[i].collider;
        }
        return colliders;
    }
    #endregion
}
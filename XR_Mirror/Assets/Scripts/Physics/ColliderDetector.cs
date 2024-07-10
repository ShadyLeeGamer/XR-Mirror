using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ColliderDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] LayerMask mask;
    [SerializeField] bool ignoreThis;
    [SerializeField] Collider manualCollider;
    SphereCollider sphereCollider;
    BoxCollider boxCollider;
    CapsuleCollider capsuleCollider;

    [SerializeField] bool onEnable;
    [SerializeField] bool signalEmpty = true;

    [Header("Update")]
    [SerializeField] bool update = true;
    [SerializeField] float updateDelay;
    [SerializeField] bool optimiseForUpdate;

    delegate Collider[] TargetDetection();
    TargetDetection targetDetectionManual, targetDetectionUpdate;

    public event Action<Collider[]> OnDetectedColliders;

    protected virtual void Awake()
    {
        if (manualCollider)
        {
            SetupWithCollider(manualCollider);
        }
        else if (TryGetComponent(out Collider collider))
        {
            SetupWithCollider(collider);
        }
    }

    protected virtual void OnEnable()
    {
        if (onEnable)
        {
            StartCoroutine(ManualNextFrame());
        }

        if (update)
        {
            StartCoroutine(UpdateDetection());
        }
    }

    protected virtual void OnDisable()
    {
        if (update)
        {
            StopCoroutine(UpdateDetection());
        }
    }

    protected virtual void Start() { }

    protected virtual void Update() { }

    IEnumerator ManualNextFrame()
    {
        yield return null; // Next frame, after all listeners subscribed to event
        ManualDetectAll();
    }

    void SetupWithCollider(Collider collider)
    {
        sphereCollider = SetupWithCollider<SphereCollider>(collider, TargetSphereDetection, TargetSphereDetectionOptimised);
        if (sphereCollider)
            return;
        
        boxCollider = SetupWithCollider<BoxCollider>(collider, TargetBoxDetection, TargetBoxDetectionOptimised);
        if (boxCollider)
            return;
        
        capsuleCollider = SetupWithCollider<CapsuleCollider>(collider, TargetCapsuleDetection, TargetCapsuleDetectionOptimised);
        if (capsuleCollider)
            return;

        Debug.LogError("No suitable collider found!");
    }

    T SetupWithCollider<T>(Collider refCollider,
        TargetDetection normalDetection,
        TargetDetection optimisedDetection) where T : Collider
    {
        T collider = refCollider as T;
        if (collider)
        {
            targetDetectionManual = normalDetection;
            targetDetectionUpdate = optimiseForUpdate
                ? optimisedDetection
                : normalDetection;
        }
        return collider;
    }

    IEnumerator UpdateDetection()
    {
        while (true)
        {
            targetDetectionUpdate();
            yield return new WaitForSeconds(updateDelay);
        }
    }

    public Collider[] ManualDetectAll()
    {
        return targetDetectionManual();
    }

    #region Optimised
    Collider[] TargetSphereDetectionOptimised()
    {
        (Vector3 center, float radius) = CalculateSphere();
        return SphereDetectCollidersOptimised(center, radius, mask);
    }
    protected abstract Collider[] SphereDetectCollidersOptimised(Vector3 center, float radius, LayerMask mask);

    Collider[] TargetBoxDetectionOptimised()
    {
        (Vector3 center, Vector3 halfExtents) = CalculateBox();
        return BoxDetectCollidersOptimised(center, halfExtents, transform.rotation, mask);
    }
    protected abstract Collider[] BoxDetectCollidersOptimised(Vector3 center, Vector3 halfExtents, Quaternion rotation, LayerMask mask);

    Collider[] TargetCapsuleDetectionOptimised()
    {
        (Vector3 point0, Vector3 point1, float radius) = CalculateCapsule();
        return CapsuleDetectCollidersOptimised(point0, point1, radius, mask);
    }
    protected abstract Collider[] CapsuleDetectCollidersOptimised(Vector3 point0, Vector3 point1, float radius, LayerMask mask);
    #endregion

    #region Normal
    Collider[] TargetSphereDetection()
    {
        (Vector3 center, float radius) = CalculateSphere();
        Collider[] colliders = SphereDetectColliders(center, radius, mask);
        return FinalTargets(colliders);
    }
    protected abstract Collider[] SphereDetectColliders(Vector3 center, float radius, LayerMask mask);

    Collider[] TargetBoxDetection()
    {
        (Vector3 center, Vector3 halfExtents) = CalculateBox();
        Collider[] colliders = BoxDetectColliders(center, halfExtents, transform.rotation, mask);
        return FinalTargets(colliders);
    }
    protected abstract Collider[] BoxDetectColliders(Vector3 center, Vector3 halfExtents, Quaternion rotation, LayerMask mask);

    Collider[] TargetCapsuleDetection()
    {
        (Vector3 point0, Vector3 point1, float radius) = CalculateCapsule();
        Collider[] colliders = CapsuleDetectColliders(point0, point1, radius, mask);
        return FinalTargets(colliders);
    }
    protected abstract Collider[] CapsuleDetectColliders(Vector3 point0, Vector3 point1, float radius, LayerMask mask);
    #endregion

    protected Collider[] FinalTargets(Collider[] colliders)
    {
        if (colliders.Length > 0)
        {
            if (ignoreThis)
            {
                HashSet<Collider> filteredColliders = new(colliders);
                foreach (var filteredCollider in filteredColliders)
                {
                    // Exclude this game object
                    if (filteredCollider.transform.root == transform.root)
                    {
                        filteredColliders.Remove(filteredCollider);
                        break;
                    }
                }
                colliders = filteredColliders.ToArray();
            }

            OnDetectedColliders?.Invoke(colliders);
        }
        else if (signalEmpty)
        {
            OnDetectedColliders?.Invoke(colliders);
        }

        return colliders;
    }

    #region Helpers
    /// <summary>
    /// Calculates sphere collider shape info relative to transform
    /// </summary>
    /// <returns>center, radius</returns>
    (Vector3, float) CalculateSphere()
    {
        Vector3 center = transform.TransformPoint(sphereCollider.center);
        float radius = transform.TransformRadius(sphereCollider.radius);
        return (center, radius);
    }

    /// <summary>
    /// Calculates box collider shape info relative to transform
    /// </summary>
    /// <returns>center, halfExtents</returns>
    (Vector3, Vector3) CalculateBox()
    {
        Vector3 center = transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = Vector3.Scale(boxCollider.size * 0.5f, transform.lossyScale);
        return (center, halfExtents);
    }

    /// <summary>
    /// Calculates capsule collider shape info relative to transform
    /// </summary>
    /// <returns>bottom, top, radius</returns>
    (Vector3, Vector3, float) CalculateCapsule()
    {
        var dir = new Vector3 { [capsuleCollider.direction] = 1 };
        float offset = capsuleCollider.height / 2 - capsuleCollider.radius;
        
        Vector3 bottom = transform.TransformPoint(capsuleCollider.center - dir * offset);
        Vector3 top = transform.TransformPoint(capsuleCollider.center + dir * offset);
        float radius = transform.TransformCapsuleRadius(capsuleCollider.radius, capsuleCollider.direction);
        return (bottom, top, radius);
    }
    #endregion
}

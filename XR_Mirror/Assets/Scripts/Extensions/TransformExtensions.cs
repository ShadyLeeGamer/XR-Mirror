using System.Linq;
using UnityEngine;

public static class TransformExtensions
{
    public static float TransformRadius(this Transform transform, float radius)
    {
        Vector3 scaled = transform.TransformVector(Vector3.one * radius);
        float max = Enumerable.Range(0, 3)
            .Select(xyz => scaled[xyz])
            .Select(Mathf.Abs).Max();// Select largest scale
        return max;
    }

    public static float TransformCapsuleRadius(this Transform transform, float radius, int direction)
    {
        Vector3 scaled = transform.TransformVector(Vector3.one * radius);
        float max = Enumerable.Range(0, 3)
            // Exclude axis in height direction
            .Select(xyz => xyz == direction ? 0 : scaled[xyz])
            .Select(Mathf.Abs).Max();// Select largest scale
        return max;
    }
}

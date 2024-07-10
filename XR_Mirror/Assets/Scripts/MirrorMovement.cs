using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform mirror;

    private void Update()
    {
        Vector3 localTarget = mirror.InverseTransformPoint(target.position);
        transform.position = mirror.TransformPoint(new Vector3(localTarget.x, localTarget.y, -localTarget.z));

        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localTarget.x, localTarget.y, localTarget.z));
        transform.LookAt(lookAtMirror);
    }
}

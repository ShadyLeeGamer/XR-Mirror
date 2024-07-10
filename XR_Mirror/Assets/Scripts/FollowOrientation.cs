using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FollowOrientation : MonoBehaviour
{
    [SerializeField] Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}

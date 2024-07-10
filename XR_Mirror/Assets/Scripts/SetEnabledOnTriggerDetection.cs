using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnabledOnTriggerDetection : MonoBehaviour
{
    [SerializeField] LayerMask targetMask;
    [SerializeField] MonoBehaviour component;
    [SerializeField] bool invertSet;
    PointerEvent pe;
    int targetCount;

    private void OnTriggerEnter(Collider other)
    {
        if ((targetMask & (1 << other.gameObject.layer)) != 0)
        {
            targetCount++;
            UpdateSetObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetMask & (1 << other.gameObject.layer)) != 0)
        {
            targetCount--;
            UpdateSetObject();
        }
    }

    void UpdateSetObject()
    {
        bool value = targetCount > 0;
        if (invertSet)
        {
            value = !value;
        }
        component.enabled = value;
    }
}

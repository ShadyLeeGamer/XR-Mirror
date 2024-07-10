using DG.Tweening;
using UnityEngine;

[CreateAssetMenu()]
public class InteractableFeedbackTransitionConfig : ScriptableObject
{
    public float duration;
    public Ease ease;

    [Header("Scale")]
    public float hoverScale;
    public float selectScale;

    [Header("Colour")]
    [ColorUsage(false)] public Color hoverColour;
    [ColorUsage(false)] public Color selectColour;
}

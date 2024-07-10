using DG.Tweening;
using Oculus.Interaction;
using UnityEngine;

public class InteractableFeedback : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))]
    UnityEngine.Object _interactable;
    IInteractableView interactable;

    [SerializeField] MeshRenderer[] meshRenderers;
    [SerializeField] InteractableFeedbackTransitionConfig config;

    Vector3[] defaultScales;
    Color defaultColour;

    private void Awake()
    {
        interactable = _interactable as IInteractableView;
        if (interactable == null)
        {
            Debug.LogError("Interactable is not assigned");
            enabled = false;
            return;
        }

        // Default values
        defaultScales = new Vector3[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            defaultScales[i] = meshRenderers[i].transform.localScale;
        }
        defaultColour = meshRenderers[0].material.color;
    }

    private void OnEnable()
    {
        interactable.WhenStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        interactable.WhenStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        switch (args.NewState)
        {
            case InteractableState.Normal:
                if (args.PreviousState == InteractableState.Hover)
                {
                    HoverExit();
                }
                break;
            case InteractableState.Hover:
                if (args.PreviousState == InteractableState.Normal)
                {
                    HoverEnter();
                }
                else if (args.PreviousState == InteractableState.Select)
                {
                    HoverExit();
                }
                break;
            case InteractableState.Select:
                if (args.PreviousState == InteractableState.Hover)
                {
                    Select();
                }
                break;
        }
    }

    void HoverEnter() => Tween(config.hoverScale, config.hoverColour);

    void Select() => Tween(config.selectScale, config.selectColour);

    void HoverExit() => Tween(defaultScales, defaultColour);

    void Tween(float scaleMultiplier, Color endColour)
    {
        var endScales = new Vector3[defaultScales.Length];
        for (int i = 0; i < endScales.Length; i++)
        {
            endScales[i] = defaultScales[i] * scaleMultiplier;
        }
        Tween(endScales, endColour);
    }

    void Tween(Vector3[] endScales, Color endColour)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].transform.DOScale(endScales[i], config.duration).SetEase(config.ease);
            meshRenderers[i].material.DOColor(endColour, config.duration).SetEase(config.ease);
        }
    }
}

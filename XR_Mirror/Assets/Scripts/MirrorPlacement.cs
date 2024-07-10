using Oculus.Interaction;
using UnityEngine;

public class MirrorPlacement : MonoBehaviour
{
    [SerializeField] GameObject webcamPlace;
    [SerializeField] GameObject mirrorSetup;
    [SerializeField] Transform orientation;
    [SerializeField] PokeInteractable placeButton;

    private void OnEnable()
    {
        placeButton.WhenStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        placeButton.WhenStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        switch (args.NewState)
        {
            case InteractableState.Select:
                if (args.PreviousState == InteractableState.Hover)
                {
                    Place();
                }
                break;
        }
    }

    void Place()
    {
        mirrorSetup.transform.position = orientation.position;
        mirrorSetup.transform.eulerAngles = new Vector3(-orientation.eulerAngles.x, orientation.eulerAngles.y, orientation.eulerAngles.z);
        Destroy(webcamPlace);
        mirrorSetup.SetActive(true);
    }
}

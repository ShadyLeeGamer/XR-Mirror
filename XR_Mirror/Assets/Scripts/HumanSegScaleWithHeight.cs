using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSegScaleWithHeight : MonoBehaviour
{
    [SerializeField] Transform quad;
    [SerializeField] Observer<float> widthOffset;

    float lastScreenWidth, lastScreenHeight;

    private void Awake()
    {
        UpdateQuadWidth();
    }

    private void OnEnable()
    {
        widthOffset.OnValueChanged += WidthOffset_OnValueChanged;
    }

    private void OnDisable()
    {
        widthOffset.OnValueChanged -= WidthOffset_OnValueChanged;
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateQuadWidth();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    public void UpdateQuadWidth()
    {
        float quadHeight = quad.localScale.y;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float ratio = screenWidth / screenHeight;
        float quadWidth = quadHeight * ratio;
        quadWidth += widthOffset;
        quad.localScale = new Vector3(quadWidth, quadHeight);
    }

    private void WidthOffset_OnValueChanged(float _)
    {
        UpdateQuadWidth();
    }
}

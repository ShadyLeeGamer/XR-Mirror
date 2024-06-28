using UnityEngine;

public class CamDisplay : MonoBehaviour
{
    const int
        WIDTH = 1920,
        HEIGHT = 1080;

    [SerializeField] Material mat;

    Texture2D tex2d;
    CamFeedReceiver feedReceiver;

    public static CamDisplay Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        tex2d = new(WIDTH, HEIGHT);
    }

    void Start()
    {
        feedReceiver = CamFeedReceiver.Instance;
        feedReceiver.OnReceived += Refresh;
    }

    void Refresh(byte[] frameBytes)
    {
        tex2d.LoadImage(frameBytes); // Deserialise data
        mat.mainTexture = tex2d;
    }
}
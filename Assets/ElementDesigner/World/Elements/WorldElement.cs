using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; protected set; }

    private Canvas infoCanvas;
    private Text infoText;
    private TrailRenderer trail;
    private ParticleSystem particles;
    private Light bodyLight;
    private MeshRenderer bodyMR;
    private Transform bodyTransform;

    private bool initialized = false;

    void VerifyInitialize()
    {
        if (initialized)
            return;

        bodyTransform = transform.Find("Body");
        Assertions.AssertNotNull(bodyTransform, "bodyTransform");
        bodyLight = bodyTransform.Find("Light").GetComponent<Light>();
        var infoCanvasTransform = bodyTransform.Find("InfoCanvas");
        infoCanvas = infoCanvasTransform.GetComponent<Canvas>();

        infoText = infoCanvasTransform.Find("Text").GetComponent<Text>();

        trail = GetComponent<TrailRenderer>();
        particles = GetComponent<ParticleSystem>();

        bodyMR = bodyTransform.GetComponent<MeshRenderer>();

        trail.startWidth = bodyTransform.lossyScale.magnitude * .25f;
        initialized = true;
    }
    protected void Start()
    {
        VerifyInitialize();
    }

    void Update()
    {
        if (infoText != null && infoCanvas != null)
        {
            var signCanvasRect = infoCanvas.GetComponent<RectTransform>();
            var body = transform.Find("Body");

            var dist = Vector3.Distance(transform.position, Camera.main.transform.position) * .075f;
            signCanvasRect.localScale = new Vector3(1 + dist, 1 + dist, 1 + dist) * (1 / body.localScale.magnitude);

            infoCanvas.gameObject.SetActive(dist > 5);
        }
    }

    private void SetColor(Color newColor)
    {
        VerifyInitialize();
        bodyLight.color = newColor;
        bodyMR.material.color = newColor;
        bodyMR.material.SetColor("_EmissionColor", newColor);

        trail.material.color = newColor;
        trail.material.SetColor("_EmissionColor", newColor);
    }

    public void SetData(Element elementData) => Data = elementData;
}
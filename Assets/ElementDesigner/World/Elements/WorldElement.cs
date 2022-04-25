using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; protected set; }

    private Canvas infoCanvas;
    protected Text infoText;
    private ParticleSystem particles;
    private Light bodyLight;
    private MeshRenderer bodyMR;
    protected Transform bodyTransform;
    // TODO: maybe 'charge' could be determined by weight?
    protected float charge = 0;
    protected float massMultiplier = 1;

    private bool initialized = false;

    protected void VerifyInitialize()
    {
        if (initialized)
            return;

        bodyTransform = transform.Find("Body");
        Assertions.AssertNotNull(bodyTransform, "bodyTransform");
        bodyLight = bodyTransform.Find("Light").GetComponent<Light>();
        var infoCanvasTransform = bodyTransform.Find("InfoCanvas");
        infoCanvas = infoCanvasTransform.GetComponent<Canvas>();

        infoText = infoCanvasTransform.Find("Text").GetComponent<Text>();


        particles = GetComponent<ParticleSystem>();

        bodyMR = bodyTransform.GetComponent<MeshRenderer>();


        initialized = true;
    }
    protected virtual void Start()
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


    }

    public void SetData(Element elementData) => Data = elementData;
}
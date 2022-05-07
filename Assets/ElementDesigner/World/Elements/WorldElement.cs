using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; private set; }

    private Transform bodyTransform;
    public Transform BodyTransform
    {
        get
        {
            if (bodyTransform == null)
                bodyTransform = transform.Find("Body");
            if (bodyTransform == null)
                throw new ApplicationException("WorldElement requires a BodyTransform, but none was present");

            return bodyTransform;
        }
    }
    // TODO: maybe 'charge' could be determined by weight?
    public float Charge { get; private set; }
    public Color Color { get; private set; }
    public float MassMultiplier { get; private set; }

    protected string infoString { get; private set; }
    protected Text infoText { get; private set; }

    private Canvas infoCanvas;
    private Light bodyLight;
    private MeshRenderer bodyMR;

    protected bool initialized = false;

    protected virtual void VerifyInitialize()
    {
        if (initialized)
            return;

        bodyMR = BodyTransform?.GetComponent<MeshRenderer>();
        Assertions.AssertNotNull(bodyMR, "bodyMR (MeshRenderer)");
        bodyLight = BodyTransform?.Find("Light").GetComponent<Light>();
        Assertions.AssertNotNull(bodyLight, "bodyLight");

        var infoCanvasTransform = BodyTransform.Find("InfoCanvas");
        infoCanvas = infoCanvasTransform?.GetComponent<Canvas>();
        Assertions.AssertNotNull(infoCanvas, "infoCanvas");

        var mainCameraTransform = Camera.main.transform;
        var uiCamera = mainCameraTransform?.Find("UICamera")?.GetComponent<Camera>();
        Assertions.AssertNotNull(uiCamera, "uiCamera");
        infoCanvas.worldCamera = uiCamera;

        infoText = infoCanvasTransform?.Find("Text").GetComponent<Text>();
        Assertions.AssertNotNull(infoText, "infoText");

        initialized = true;
    }
    protected virtual void Start()
    {
        VerifyInitialize();
    }

    protected virtual void Update()
    {
        var signCanvasRect = infoCanvas.GetComponent<RectTransform>();
        var body = transform.Find("Body");

        var dist = Vector3.Distance(transform.position, Camera.main.transform.position) * .075f;
        signCanvasRect.localScale = new Vector3(1 + dist, 1 + dist, 1 + dist) * (1 / body.localScale.magnitude * .5f);

        infoCanvas.gameObject.SetActive(dist > 2);
    }

    protected virtual void SetColor(Color newColor)
    {
        VerifyInitialize();
        bodyLight.color = newColor * 2;
        bodyMR.material.color = newColor;
        bodyMR.material.SetColor("_EmissionColor", newColor);

        Color = newColor;
    }

    public void SetData(Element element)
    {
        VerifyInitialize();
        Data = element;

        var pCharge = element.Charge;
        Charge = pCharge;

        var ch = element.Charge;
        var chargeSign = ch < 0 ? "-" : ch == 0 ? "" : "+";
        infoText.text = element.ShortName + chargeSign;

        MassMultiplier = element.Weight;

        if (element.ElementType != ElementType.Particle)
            SetColor(element.Color);
    }
}
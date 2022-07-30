using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; private set; } = new Element();

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

    private Canvas signCanvas;
    private Light bodyLight;
    private MeshRenderer bodyMR;
    private Interact interact;

    protected bool initialized = false;

    protected virtual void VerifyInitialize()
    {
        if (initialized)
            return;

        bodyMR = BodyTransform?.GetComponent<MeshRenderer>();
        Assertions.AssertNotNull(bodyMR, "bodyMR (MeshRenderer)");
        bodyLight = BodyTransform?.Find("Light")?.GetComponent<Light>();
        // Assertions.AssertNotNull(bodyLight, "bodyLight");

        var rotorTransform = BodyTransform.Find("Rotor");
        var infoCanvasTransform = rotorTransform?.Find("InfoCanvas");
        signCanvas = infoCanvasTransform?.GetComponent<Canvas>();
        Assertions.AssertNotNull(signCanvas, "signCanvas");

        var mainCameraTransform = Camera.main.transform;
        var uiCamera = mainCameraTransform?.Find("UICamera")?.GetComponent<Camera>();
        Assertions.AssertNotNull(uiCamera, "uiCamera");
        signCanvas.worldCamera = uiCamera;

        infoText = infoCanvasTransform?.Find("Text").GetComponent<Text>();
        Assertions.AssertNotNull(infoText, "infoText");

        interact = GetComponent<Interact>();
        Assertions.AssertNotNull(interact, "interact");

        initialized = true;
    }
    protected virtual void Start()
    {
        VerifyInitialize();
    }

    protected virtual void Update()
    {
        var signCanvasRect = signCanvas.GetComponent<RectTransform>();
        var body = transform.Find("Body");

        var dist = Vector3.Distance(transform.position, Camera.main.transform.position) * .075f;
        signCanvasRect.localScale = new Vector3(1 + dist, 1 + dist, 1 + dist) * (1 / body.localScale.magnitude * .5f);

        infoText.gameObject.SetActive(dist > 5);
    }

    protected virtual void SetColor(Color newColor)
    {
        VerifyInitialize();

        var emphasisColor = Data.ElementType != ElementType.Particle ? newColor.Emphasise() : newColor;

        if (bodyLight != null)
            bodyLight.color = emphasisColor;

        bodyMR.material.SetColor("_EmissionColor", emphasisColor);

        bodyMR.material.color = newColor;
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

        interact.ElementDisplay.SetData(element);

        if (element.ElementType != ElementType.Particle)
            SetColor(element.Color);
    }
}
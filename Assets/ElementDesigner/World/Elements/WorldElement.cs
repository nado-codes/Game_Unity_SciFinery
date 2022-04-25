using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; private set; }
    private Canvas infoCanvas;
    protected Text infoText;
    private Light bodyLight;
    private MeshRenderer bodyMR;

    private Transform bodyTransform;
    public Transform BodyTransform
    {
        get
        {
            if (bodyTransform == null)
                bodyTransform = transform.Find("bodyTransform");
            if (bodyTransform == null)
                throw new ApplicationException("WorldElement requires a BodyTransform, but none was present");

            return bodyTransform;
        }
    }
    // TODO: maybe 'charge' could be determined by weight?
    protected float charge = 0;
    public float Charge => charge;
    protected string chargeString = "";
    public float MassMultiplier { get; private set; }

    private bool initialized = false;

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
        infoText = infoCanvasTransform?.Find("Text").GetComponent<Text>();
        Assertions.AssertNotNull(infoText, "infoText");

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

    protected virtual void SetColor(Color newColor)
    {
        VerifyInitialize();
        bodyLight.color = newColor;
        bodyMR.material.color = newColor;
        bodyMR.material.SetColor("_EmissionColor", newColor);
    }

    public void SetData(Element elementData)
    {
        Data = elementData;

        var pCharge = elementData.Charge;
        charge = pCharge;

        chargeString = pCharge == 0 ? string.Empty : pCharge < 0 ? "-" : "+";
        infoText.text = chargeString;

        MassMultiplier = elementData.Weight;
    }
}
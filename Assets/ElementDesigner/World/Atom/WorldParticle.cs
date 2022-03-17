using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public enum ParticleType { Proton, Neutron, Electron }

public class WorldParticle : WorldElement
{
    private Vector3 velocity = Vector3.zero;

    // TODO: maybe 'charge' could be determined by weight?
    public int charge = 0;

    private Canvas infoCanvas;
    private Text infoText;
    private TrailRenderer trail;
    private Light bodyLight;
    private MeshRenderer bodyMR;

    public float massMultiplier = 1;
    private bool initialized = false;

    void VerifyInitialize()
    {
        if (initialized)
            return;

        var body = transform.Find("Body");
        bodyLight = body.Find("Light").GetComponent<Light>();
        var infoCanvasTransform = body.Find("InfoCanvas");
        infoCanvas = infoCanvasTransform.GetComponent<Canvas>();

        infoText = infoCanvasTransform.Find("Text").GetComponent<Text>();

        trail = GetComponent<TrailRenderer>();

        bodyMR = body.GetComponent<MeshRenderer>();

        trail.startWidth = body.lossyScale.magnitude * .25f;
        initialized = true;
    }
    protected void Start()
    {
        VerifyInitialize();
    }

    protected void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (infoText != null && infoCanvas != null)
        {
            var signCanvasRect = infoCanvas.GetComponent<RectTransform>();
            var body = transform.Find("Body");

            var dist = Vector3.Distance(transform.position, Camera.main.transform.position) * .075f;
            signCanvasRect.localScale = new Vector3(1 + dist, 1 + dist, 1 + dist) * (1 / body.localScale.magnitude);

            infoCanvas.gameObject.SetActive(dist > 5);
        }

        // Apply charges
        var worldParticles = Editor.Particles.Where(x => x != this);

        var effectiveForce = Vector3.zero;
        worldParticles.ToList().ForEach(x =>
        {
            var effectiveCharge = x.charge * charge;
            var xBody = x.transform.Find("Body");
            var body = transform.Find("Body");
            var massOffset = 1 / (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * massMultiplier;
            var distanceOffset = 10 * (1 / Vector3.Distance(xBody.transform.position, transform.position));

            // .. comment this out to enable repulsive forces
            //effectiveCharge = effectiveCharge == 1 ? -1 : effectiveCharge;

            var dirTo = transform.position - x.transform.position;
            effectiveForce += dirTo * effectiveCharge * massOffset * distanceOffset;
        });

        velocity += effectiveForce * Time.deltaTime * .5f;
        trail.time = 10 * (1 / velocity.magnitude);
    }

    public void SetColor(Color newColor)
    {
        VerifyInitialize();
        bodyLight.color = newColor;
        bodyMR.material.color = newColor;
        bodyMR.material.SetColor("_EmissionColor", newColor);

        // var trailColorKeys = new GradientColorKey[] { new GradientColorKey(Color.green, 0) };
        // var trailAlphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(0, 0) };
        trail.material.color = newColor;
        trail.material.SetColor("_EmissionColor", newColor);

        // trail.colorGradient.SetKeys(trailColorKeys, trailAlphaKeys);
    }

    public void SetInfoText(string newInfoText)
    {
        VerifyInitialize();
        infoText.text = newInfoText;
    }
}
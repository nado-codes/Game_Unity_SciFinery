using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public enum ParticleType { Proton, Neutron, Electron }

public class WorldParticle : WorldElement
{
    private WorldElementMotor motor;
    private WorldElementMotor Motor
    {
        get
        {
            if (motor == null)
                motor = GetComponent<WorldElementMotor>();
            if (motor == null)
                throw new ApplicationException("WorldParticle requires a WorldElementMotor in order to work properly.");

            return motor;
        }
    }

    private TrailRenderer trail;

    protected override void Start()
    {
        base.Start();
        trail = GetComponent<TrailRenderer>();
        trail.startWidth = bodyTransform.lossyScale.magnitude * .25f;
    }

    void Update()
    {
        trail.time = 10 * Mathf.Min((1 / Motor.Velocity.magnitude), 1f);
    }

    // TODO: Everything is a WorldElement but not every WorldElement can move or has a trail
    // WorldElementMotor - For movement
    // WorldElementTrail - For trails

    protected void SetColor(Color newColor)
    {
        trail.material.color = newColor;
        trail.material.SetColor("_EmissionColor", newColor);
    }

    public void SetParticleData<T>(T particleData) where T : Particle
    {
        base.VerifyInitialize();

        bodyTransform.localScale = new Vector3(particleData.Size, particleData.Size, particleData.Size);

        ColorUtility.TryParseHtmlString(particleData.Color, out Color particleColor);
        SetColor(particleColor);

        var newInfoText = particleData.Charge == 0 ? string.Empty : particleData.Charge < 0 ? "-" : "+";
        infoText.text = newInfoText;

        charge = particleData.Charge;
        massMultiplier = particleData.Weight;
    }
}
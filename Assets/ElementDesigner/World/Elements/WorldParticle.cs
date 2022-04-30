using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public class WorldParticle : WorldElement
{
    protected new bool initialized = false;
    private WorldElementMotor motor;
    private WorldElementMotor Motor
    {
        get
        {
            if (motor == null)
                motor = GetComponent<WorldElementMotor>();
            if (motor == null)
                throw new ApplicationException("WorldParticle requires a WorldElementMotor to work properly. Please add one first.");

            return motor;
        }
    }

    protected override void Start()
    {
        base.Start();
        VerifyInitialize();
    }

    void Update()
    {

    }

    // TODO: Everything is a WorldElement but not every WorldElement can move or has a trail
    // WorldElementMotor - For movement
    // WorldElementTrail - For trails

    public void SetParticleData(Particle particleData)
    {
        base.VerifyInitialize();
        base.SetData(particleData);

        var pSize = particleData.Size;
        BodyTransform.localScale = new Vector3(pSize, pSize, pSize);

        ColorUtility.TryParseHtmlString(particleData.Color, out Color particleColor);
        SetColor(particleColor);
    }
}
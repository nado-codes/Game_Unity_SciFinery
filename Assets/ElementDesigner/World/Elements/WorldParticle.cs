using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public enum ParticleType { Proton, Neutron, Electron }

public class WorldParticle : WorldElement
{
    void Update()
    {
        trail.time = 10 * Mathf.Min((1 / velocity.magnitude), 1f);
    }

    // TODO: Everything is a WorldElement but not every WorldElement can move or has a trail
    // WorldElementMotor - For movement
    // WorldElementTrail - For trails

    public void SetParticleData<T>(T particleData) where T : Particle
    {
        VerifyInitialize();

        bodyTransform.localScale = new Vector3(particleData.Size, particleData.Size, particleData.Size);

        ColorUtility.TryParseHtmlString(particleData.Color, out Color particleColor);
        SetColor(particleColor);

        var newInfoText = particleData.Charge == 0 ? string.Empty : particleData.Charge < 0 ? "-" : "+";
        infoText.text = newInfoText;

        Charge = particleData.Charge;
        massMultiplier = particleData.Weight;
    }
}
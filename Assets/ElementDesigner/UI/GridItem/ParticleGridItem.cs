using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParticleGridItem : ElementGridItem
{
    private Text chargeText;
    public void SetData(Particle particleData)
    {
        if (particleData == null)
            throw new ApplicationException("Expected particleData in call to ParticleGridItem.SetAtomData, got null");

        base.SetData(particleData);

        if (ActiveLayout.name != "Layout_Particle")
            throw new ApplicationException("ParticleGridItem requires ActiveLayout \"Layout_Particle\" in call to SetData");

        var icon = ActiveLayout.Find("Icon")?.GetComponent<Image>();

        if (icon == null)
            throw new ApplicationException("Expected an icon in call to ParticleGridItem.SetData, got null");

        var validColor = ColorUtility.TryParseHtmlString(particleData.ColorHex, out Color particleColor);

        if (!validColor)
            throw new ApplicationException($"Invalid color for particle in call to ParticleGridItem.SetData. Must be hex. ({particleData.ColorHex})");

        icon.color = particleColor;

        var chargePanelTransform = ActiveLayout.Find("panelCharge");
        var chargeSign = particleData.Charge < 0 ? "-" : string.Empty;
        chargeText = chargePanelTransform.Find("Charge")?.GetComponent<Text>();
        chargeText.text = chargeSign + particleData.Charge;
    }
}
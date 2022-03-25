using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParticleGridItem : ElementGridItem<Particle>
{
    private Text chargeText;
    public override void SetData(Particle particleData)
    {
        if (particleData == null)
            throw new ApplicationException("Expected particleData in call to ParticleGridItem.SetAtomData, got null");

        base.SetData(particleData);

        var nameWithoutVowels = new string(particleData.Name.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();

        numberText = ActiveLayout.Find("Number")?.GetComponent<Text>();
        numberText.text = particleData.Id.ToString();

        var chargePanelTransform = ActiveLayout.Find("panelCharge");
        var chargeSign = particleData.Charge < 0 ? "-" : string.Empty;
        chargeText = chargePanelTransform.Find("Charge")?.GetComponent<Text>();
        chargeText.text = chargeSign + particleData.Charge;

        // TODO: add "chargeText" to display charge
        // weightText.text = particleData.Charge.ToString() + ".00";
    }
}
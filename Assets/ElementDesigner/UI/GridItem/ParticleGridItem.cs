using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParticleGridItem : ElementGridItem<Particle>
{
    public override void SetData(Particle particleData)
    {
        base.SetData(particleData);

        if (particleData == null)
            throw new ApplicationException("Expected particleData in call to ParticleGridItem.SetAtomData, got null");

        var nameWithoutVowels = new string(particleData.Name.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        nameText.text = newShortName + (particleData.Charge > 0 ? "+" : particleData.Charge < 0 ? "-" : string.Empty);

        numberText.text = particleData.Id.ToString();
        nameText.text = particleData.Name;
        // TODO: add "chargeText" to display charge
        // weightText.text = particleData.Charge.ToString() + ".00";

        SetActive(true);

    }
}
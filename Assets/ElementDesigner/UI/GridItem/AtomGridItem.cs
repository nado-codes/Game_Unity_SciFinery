using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class AtomGridItem : ElementGridItem
{
    public override void SetActive(bool active)
    {
        VerifyInitialize();

        numberText.gameObject.SetActive(active);
        shortNameText.gameObject.SetActive(active);
        nameText.gameObject.SetActive(active);
        weightText.gameObject.SetActive(active);

        base.SetActive(active);
    }

    public void SetData(Atom atomData)
    {
        VerifyInitialize();

        base.SetData(atomData);

        if (atomData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in PeriodicTableGridItem, got null");

        numberText.text = atomData.Id.ToString();
        shortNameText.text = atomData.ShortName;
        nameText.text = atomData.Name;
        weightText.text = atomData.Weight.ToString() + ".00";

        SetActive(true);

    }

    protected override void VerifyInitialize()
    {
        if (initialized)
            return;

        base.VerifyInitialize();



        if (ActiveLayout == null)
            throw new NullReferenceException("No layout was set in call to VerifyInitialize");

        numberText = ActiveLayout.Find("Number")?.GetComponent<Text>();
        Assertions.AssertNotNull(numberText, "numberText");
        shortNameText = ActiveLayout.Find("ShortName")?.GetComponent<Text>();
        Assertions.AssertNotNull(shortNameText, "shortNameText");
        weightText = ActiveLayout.Find("Weight")?.GetComponent<Text>();
        Assertions.AssertNotNull(weightText, "weightText");
    }
}

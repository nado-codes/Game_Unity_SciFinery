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
        try
        {
            VerifyInitialize();

            Assertions.AssertNotNull(numberText, "numberText");
            Assertions.AssertNotNull(shortNameText, "shortNameText");
            Assertions.AssertNotNull(nameText, "nameText");
            Assertions.AssertNotNull(weightText, "weightText");

            numberText.gameObject.SetActive(active);
            shortNameText.gameObject.SetActive(active);
            nameText.gameObject.SetActive(active);
            weightText.gameObject.SetActive(active);
        }
        catch (Exception e)
        {
            //cross.SetActive(true);
            throw e;
        }

        base.SetActive(active);
    }

    public void SetData(Atom atomData)
    {
        base.SetData(atomData);
        VerifyInitialize();

        try
        {
            if (atomData == null)
                throw new ApplicationException("Expected atomData in call to SetAtomData in PeriodicTableGridItem, got null");

            numberText.text = atomData.Id.ToString();
            shortNameText.text = atomData.ShortName;
            nameText.text = atomData.Name;
            weightText.text = atomData.Weight.ToString() + ".00";
        }
        catch (Exception e)
        {
            //cross.SetActive(true);
            throw e;
        }

        SetActive(true);
    }

    protected override void VerifyInitialize()
    {
        if (initialized)
            return;

        base.VerifyInitialize();

        try
        {
            if (ActiveLayout.name != "Layout_Std")
                throw new ApplicationException($"Layout must be \"Layout_Std\" (Standard), got {ActiveLayout.name}");

            numberText = ActiveLayout.Find("Number")?.GetComponent<Text>();
            Assertions.AssertNotNull(numberText, "numberText");
            shortNameText = ActiveLayout.Find("ShortName")?.GetComponent<Text>();
            Assertions.AssertNotNull(shortNameText, "shortNameText");
            weightText = ActiveLayout.Find("Weight")?.GetComponent<Text>();
            Assertions.AssertNotNull(weightText, "weightText");
        }
        catch (Exception e)
        {
            //cross.SetActive(true);
            throw e;
        }
    }
}

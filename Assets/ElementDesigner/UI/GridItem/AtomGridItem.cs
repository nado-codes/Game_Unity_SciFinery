using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class AtomGridItem : ElementGridItem
{
    // Update is called once per frame
    void Update()
    {
        /* var nameWithoutVowels = new string(nameText.text.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        shortNameText.text = newShortName; */
    }

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

        numberText.text = atomData.Number.ToString();
        shortNameText.text = atomData.ShortName;
        nameText.text = atomData.Name;
        weightText.text = atomData.Weight.ToString() + ".00";

        SetActive(true);

    }

    protected override void VerifyInitialize()
    {
        if(initialized)
            return;
        
        base.VerifyInitialize();

        numberText = ActiveLayout.Find("Number").GetComponent<Text>();
        shortNameText = ActiveLayout.Find("ShortName").GetComponent<Text>();
        weightText = ActiveLayout.Find("Weight").GetComponent<Text>();
    }
}

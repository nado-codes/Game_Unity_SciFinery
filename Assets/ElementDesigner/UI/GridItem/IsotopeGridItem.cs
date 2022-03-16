using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IsotopeGridItem : ElementGridItem
{
    Text nameText;

    protected override void Start()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();

        base.Start();
    }
    protected override void Awake()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();

        base.Awake();
    }

    public override void SetActive(bool active)
    {
        nameText.gameObject.SetActive(active);

        base.SetActive(active);
    }

    public void SetAtomData(Atom atomData)
    {
        if (atomData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in PeriodicTableGridItem, got null");

        var nameWithoutVowels = new string(atomData.Name.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        nameText.text = newShortName + (atomData.Charge > 0 ? "+" : atomData.Charge < 0 ? "-" : string.Empty);

        atom = atomData;

        SetActive(true);
    }
}
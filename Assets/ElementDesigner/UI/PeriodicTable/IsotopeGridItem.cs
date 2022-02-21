using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IsotopeGridItem : PeriodicTableGridItem
{
    Text nameText;
    Button button;

    void Start()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();
        button = GetComponent<Button>();

        SetActive(false);
    }
    void Awake()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();
        button = GetComponent<Button>();

        SetActive(false); 
    }

    public override void SetActive(bool active)
    {
        nameText.gameObject.SetActive(active);
        button.interactable = active;
    }

    public void SetAtomData(Atom atomData)
    {
        if(atomData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in PeriodicTableGridItem, got null");

        var nameWithoutVowels = new string(atomData.Name.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        nameText.text = newShortName+(atomData.Charge > 0 ? "+" : atomData.Charge < 0 ? "-" : string.Empty);

        atom = atomData;

        SetActive(true);
    }
}
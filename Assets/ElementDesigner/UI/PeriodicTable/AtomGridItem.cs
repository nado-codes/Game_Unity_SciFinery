using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class AtomGridItem : PeriodicTableGridItem
{
    Text numberText, shortNameText, nameText, weightText;
    Button button;

    private ColorBlock buttonColorsActive, buttonColorsInactive;

    private void Init()
    {
        numberText = transform.Find("Number")?.GetComponent<Text>();
            shortNameText = transform.Find("ShortName")?.GetComponent<Text>();
            nameText = transform.Find("Name")?.GetComponent<Text>();
            weightText = transform.Find("Weight")?.GetComponent<Text>();

            button = GetComponent<Button>();
            buttonColorsActive = button.colors;

            buttonColorsInactive.normalColor = button.colors.disabledColor;
            buttonColorsInactive.highlightedColor = button.colors.disabledColor;
            buttonColorsInactive.pressedColor = button.colors.disabledColor;
            buttonColorsInactive.selectedColor = button.colors.disabledColor;
    }
    public void Start()
    {
        Init();
        SetActive(false);
    }
    public void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //var protons = World.Particles.Where(p => p.charge == Particle.Charge.Positive);
        //var neutrons = World.Particles.Where(p => p.charge == Particle.Charge.None);

        // .. TODO: Get atomic number
        //numberText.text = protons.Count().ToString();

        // .. TODO: Get atomic weight
        //weightText.text = (protons.Count()+neutrons.Count()).ToString()+".00";

        var nameWithoutVowels = new string(nameText.text.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        shortNameText.text = newShortName;
    }

    public override void SetActive(bool active)
    {
        numberText.gameObject.SetActive(active);
        shortNameText.gameObject.SetActive(active);
        nameText.gameObject.SetActive(active);
        weightText.gameObject.SetActive(active);

        button.colors = active ? buttonColorsActive : buttonColorsInactive;
    }

    // TODO: TEMPORARY number setter for visualisation purposes. TO BE REMOVED
    public void SetNumber(int number) => numberText.text = number.ToString();

    public void SetAtomData(Atom atomData)
    {
        if(atomData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in PeriodicTableGridItem, got null");
        
        numberText.text = atomData.Number.ToString();
        shortNameText.text = atomData.ShortName;
        nameText.text = atomData.Name;
        weightText.text = atomData.Weight.ToString()+".00";

        atom = atomData;
        SetActive(true);
    }
}

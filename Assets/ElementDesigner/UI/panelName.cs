using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class panelName : MonoBehaviour
{
    private static panelName instance;
    Text numberText, shortNameText, nameText, weightText;

    public static Text NameText => instance.nameText;

    void Start()
    {
        if(instance == null)
            instance = this;
        else
            throw new ApplicationException("There can only be one instance of panelName");

        numberText = transform.Find("Number").GetComponent<Text>();
        shortNameText = transform.Find("ShortName").GetComponent<Text>();
        nameText = transform.Find("btnName").GetComponentInChildren<Text>();
        weightText = transform.Find("Weight").GetComponent<Text>();
    }

    void Update()
    {
        var protons = World.Particles.Where(p => p.charge == Particle.Charge.Positive);
        var neutrons = World.Particles.Where(p => p.charge == Particle.Charge.None);
        numberText.text = protons.Count().ToString();
        weightText.text = (protons.Count()+neutrons.Count()).ToString()+".00";

        var nameWithoutVowels = new string(nameText.text.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        shortNameText.text = newShortName;
    }
}

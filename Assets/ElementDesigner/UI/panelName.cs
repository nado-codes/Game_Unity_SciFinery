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
    public Atom atom;

    void Start()
    {
        if (instance == null)
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
        var protonCount = FileSystem.instance.ActiveElementAs<Atom>().ProtonCount;
        var neutronCount = FileSystem.instance.ActiveElementAs<Atom>().NeutronCount;

        numberText.text = protonCount.ToString();
        weightText.text = FileSystem.instance.ActiveElementAs<Atom>().Weight.ToString() + ".00";

        NameText.text = FileSystem.instance.ActiveElementAs<Atom>().Name;
        Editor.atomGameObject.name = "Atom" + FileSystem.instance.ActiveElementAs<Atom>().Name;

        if (FileSystem.instance.ActiveElementAs<Atom>().Charge == 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName;
        else if (FileSystem.instance.ActiveElementAs<Atom>().Charge < 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName + "-";
        else if (FileSystem.instance.ActiveElementAs<Atom>().Charge > 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName + "+";
    }

    public void SetActive(bool active)
    {
        numberText.gameObject.SetActive(active);
        shortNameText.gameObject.SetActive(active);
        nameText.gameObject.SetActive(active);
        weightText.gameObject.SetActive(active);
    }

    public void SetAtomData(Atom atomData)
    {
        if (atomData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in panelName, got null");

        numberText.text = atomData.Number.ToString();
        shortNameText.text = atomData.ShortName;
        nameText.text = atomData.Name;
        weightText.text = atomData.Weight.ToString() + ".00";

        atom = atomData;
        SetActive(true);
    }
}

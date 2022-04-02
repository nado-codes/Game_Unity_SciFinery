using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelName : MonoBehaviour
{
    private static PanelName instance;
    Text numberText, shortNameText, nameText, weightText;

    public static Text NameText => instance.nameText;
    public Element elementData;

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

    public static void Update()
    {
        /* var protonCount = FileSystem.instance.ActiveElementAs<Atom>().ProtonCount;
        var neutronCount = FileSystem.instance.ActiveElementAs<Atom>().NeutronCount;

        instance.numberText.text = protonCount.ToString();
        instance.weightText.text = FileSystem.instance.ActiveElementAs<Atom>().Weight.ToString() + ".00";

        NameText.text = FileSystem.instance.ActiveElementAs<Atom>().Name;
        Editor.atomGameObject.name = "Atom" + FileSystem.instance.ActiveElementAs<Atom>().Name;

        if (FileSystem.instance.ActiveElementAs<Atom>().Charge == 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName;
        else if (FileSystem.instance.ActiveElementAs<Atom>().Charge < 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName + "-";
        else if (FileSystem.instance.ActiveElementAs<Atom>().Charge > 0)
            shortNameText.text = FileSystem.instance.ActiveElementAs<Atom>().ShortName + "+"; */
    }

    private void setActive(bool active)
    {
        numberText.gameObject.SetActive(active);
        shortNameText.gameObject.SetActive(active);
        nameText.gameObject.SetActive(active);
        weightText.gameObject.SetActive(active);
    }

    public static void SetElementData(Element newElementData)
    {
        if (newElementData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in panelName, got null");

        if (newElementData is Atom)
            instance.setAtomData(newElementData as Atom);
        else
        {
            instance.shortNameText.text = newElementData.ShortName;
            instance.nameText.text = newElementData.Name;
            instance.weightText.text = newElementData.Weight.ToString() + ".00";
        }

        instance.elementData = newElementData;
        instance.setActive(true);
    }

    private void setAtomData(Atom atomData)
    {
        instance.numberText.text = atomData.Number.ToString();
        instance.shortNameText.text = atomData.ShortName;
        instance.nameText.text = atomData.Name;
        instance.weightText.text = atomData.Weight.ToString() + ".00";
    }
}

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

        instance.numberText.text = newElementData.Id.ToString();
        instance.shortNameText.text = newElementData.ShortName;
        instance.nameText.text = newElementData.Name;
        instance.weightText.text = newElementData.Weight + ".00";

        instance.setActive(true);
    }
}

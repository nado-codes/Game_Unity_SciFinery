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
    Text classificationText, stabilityText, chargeText;

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

        var classificationTransform = transform.Find("Classification");
        classificationText = classificationTransform.Find("Value").GetComponent<Text>();
        stabilityText = transform.Find("TextStability").GetComponent<Text>();
        chargeText = transform.Find("TextCharge").GetComponent<Text>();
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

        var finalShortName =
        newElementData.Charge < 0 ?
            newElementData.ShortName + "-" :
        newElementData.Charge == 0 ?
            newElementData.ShortName :
            newElementData.ShortName + "+";
        instance.shortNameText.text = finalShortName;

        instance.nameText.text = newElementData.Name;
        instance.weightText.text = newElementData.Weight + ".00";

        // TODO: implement classification
        // instance.classificationText.text = newElementData.Classification;
        // TODO: implement stability/radioactivity
        // instance.stabilityText = newElementData.Stability;
        instance.chargeText.text = newElementData.Charge.ToString();

        instance.setActive(true);
    }
}

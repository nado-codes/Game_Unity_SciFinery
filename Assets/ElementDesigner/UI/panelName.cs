using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelName : MonoBehaviour
{
    private static PanelName instance;
    public static PanelName Instance
    {
        get
        {
            initInstance();
            return instance;
        }
    }
    Text numberText, shortNameText, nameText, weightText;
    Text classificationText, stabilityText, chargeText;

    public static Text NameText => instance.nameText;

    private static void initInstance()
    {
        if (instance == null)
            instance = FindObjectOfType<PanelName>();
        if (instance == null)
        {
            var gameObject = new GameObject();
            gameObject.name = typeof(PanelName).FullName;
            instance = gameObject.AddComponent<PanelName>();
        }
    }
    public void VerifyInitialize()
    {
        initInstance();
        var instanceTransform = instance.transform;

        numberText = instanceTransform.Find("Number").GetComponent<Text>();
        shortNameText = instanceTransform.Find("ShortName").GetComponent<Text>();
        nameText = transform.Find("btnName").GetComponentInChildren<Text>();
        weightText = transform.Find("Weight").GetComponent<Text>();

        var classificationTransform = transform.Find("Classification");
        classificationText = classificationTransform.Find("Value").GetComponent<Text>();
        stabilityText = transform.Find("TextStability").GetComponent<Text>();
        chargeText = transform.Find("TextCharge").GetComponent<Text>();
    }
    void Start() => VerifyInitialize();

    public static void SetElementData(Element newElementData)
    {
        Instance.VerifyInitialize();
        Instance.setElementData(newElementData);
    }

    private void setElementData(Element newElementData)
    {
        if (newElementData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in panelName, got null");

        numberText.text = newElementData.Id.ToString();

        var finalShortName =
        newElementData.Charge < 0 ?
            newElementData.ShortName + "-" :
        newElementData.Charge == 0 ?
            newElementData.ShortName :
            newElementData.ShortName + "+";
        shortNameText.text = finalShortName;

        nameText.text = newElementData.Name;
        weightText.text = newElementData.Weight + ".00";

        // TODO: implement classification
        // instance.classificationText.text = newElementData.Classification;
        // TODO: implement stability/radioactivity
        // instance.stabilityText = newElementData.Stability;
        chargeText.text = newElementData.Charge.ToString();

        setActive(true);
    }

    private void setActive(bool active)
    {
        VerifyInitialize();
        numberText.gameObject.SetActive(active);
        shortNameText.gameObject.SetActive(active);
        nameText.gameObject.SetActive(active);
        weightText.gameObject.SetActive(active);
    }
}

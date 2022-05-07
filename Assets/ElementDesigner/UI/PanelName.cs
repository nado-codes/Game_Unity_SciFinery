using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    TextMeshPro compositionText;

    public static Text NameText => instance.nameText;

    private static void initInstance()
    {
        if (instance == null)
            instance = FindObjectOfType<PanelName>();
        if (instance == null)
            throw new ApplicationException("Expected an instance of PanelName, but found nothing in call to PanelName.initInstance");
    }
    public void VerifyInitialize()
    {
        initInstance();
        var instanceTransform = instance.transform;

        numberText = instanceTransform.Find("Number")?.GetComponent<Text>();
        Assertions.AssertNotNull(numberText, "numberText");
        shortNameText = instanceTransform.Find("ShortName")?.GetComponent<Text>();
        Assertions.AssertNotNull(shortNameText, "shortNameText");

        var btnNameTransform = transform.Find("btnName");
        nameText = btnNameTransform?.Find("Name")?.GetComponent<Text>();
        Assertions.AssertNotNull(nameText, "nameText");

        weightText = transform.Find("Weight")?.GetComponent<Text>();
        Assertions.AssertNotNull(weightText, "weightText");

        var classificationTransform = transform.Find("Classification");
        classificationText = classificationTransform?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(classificationText, "classificationText");

        var compositionTransform = transform.Find("Composition");
        compositionText = compositionTransform?.Find("Value")?.GetComponent<TextMeshPro>();
        Assertions.AssertNotNull(compositionText, "compositionText");

        stabilityText = transform.Find("TextStability").GetComponent<Text>();
        Assertions.AssertNotNull(stabilityText, "stabilityText");
        chargeText = transform.Find("TextCharge").GetComponent<Text>();
        Assertions.AssertNotNull(chargeText, "chargeText");
    }
    void Start() => VerifyInitialize();

    public static void SetElementData(Element element)
    {
        Instance.VerifyInitialize();

        switch (element.ElementType)
        {
            case ElementType.Atom:
                Instance.setAtomData(element as Atom);
                break;
        }

        Instance.setElementData(element);
    }

    private void setAtomData(Atom atom)
    {
        numberText.text = atom.Number.ToString();
    }

    private void setElementData(Element newElementData)
    {
        if (newElementData == null)
            throw new ApplicationException("Expected atomData in call to SetAtomData in panelName, got null");

        var ch = newElementData.Charge;
        var chargeSign = ch < 0 ? "-" : ch == 0 ? "" : "+";
        shortNameText.text = newElementData.ShortName + chargeSign;

        nameText.text = newElementData.Name;
        weightText.text = Math.Round(newElementData.Weight) + ".00";

        var uniqueChildren = newElementData.Children.GroupBy(ch => ch.Id).Select(ch => ch.First());
        var composition = uniqueChildren.Select(ch =>
        {
            var count = newElementData.Children.Count(other => other.Id == ch.Id);
            return ch.ShortName + (count > 1 ? "<sup>" + count + "</sup>" : "");
        });
        compositionText.text = string.Join("", composition);

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

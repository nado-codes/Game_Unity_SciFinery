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
            if (instance == null)
                instance = FindObjectOfType<PanelName>();
            if (instance == null)
                throw new ApplicationException("Expected an instance of PanelName, but found nothing in call to PanelName.initInstance");
            return instance;
        }
    }
    Text numberText, shortNameText, nameText, weightText;
    Text classificationText, stabilityText, chargeText;
    TextMeshPro compositionText;

    public static Text NameText => instance.nameText;

    private static void initInstance()
    {

    }
    public void VerifyInitialize()
    {
        var instanceTransform = Instance.transform;

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

        if (element == null)
            throw new NullReferenceException("Expected an element in call to SetElementData, got undefined");

        if (element.ElementType == ElementType.Atom)
            Instance.setAtomData(element as Atom);

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
        compositionText.text = WorldUtilities.GetComposition(newElementData.Children, true);

        //2n(2)
        //2x1(2) = 2
        //2x2(2) = 8
        //sqrt(8 / 2)
        var numElectrons = newElementData.Children.Count(c => c.Charge < 0);
        var outerShell = Convert.ToInt32(Math.Sqrt(numElectrons / 2) + 1);

        var maxElectrons = 2 * Math.Pow(outerShell, 2);
        var classification = numElectrons > 2 ? "Metal" : "Non-Metal";
        instance.classificationText.text = classification;
        // TODO: implement stability/radioactivity
        // instance.stabilityText = newElementData.Stability;
        chargeText.text = newElementData.Charge.ToString();

        Debug.Log("numElectrons: " + numElectrons);
        Debug.Log("outer shell: " + outerShell);
        Debug.Log("max electrons: " + maxElectrons);

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

using System.Diagnostics;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelNamev2 : MonoBehaviour
{
    private static PanelNamev2 instance;
    public static PanelNamev2 Instance
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
            instance = FindObjectOfType<PanelNamev2>();
        if (instance == null)
            throw new ApplicationException("Expected an instance of PanelName, but found nothing");
    }
    public void VerifyInitialize()
    {
        initInstance();
        var instanceTransform = instance.transform;

        numberText = instanceTransform.Find("Number")?.GetComponent<Text>();
        AssertNotNull(numberText, "numberText");
        shortNameText = instanceTransform.Find("ShortName")?.GetComponent<Text>();
        AssertNotNull(shortNameText, "shortNameText");

        var btnNameTransform = transform.Find("btnName");
        nameText = btnNameTransform?.Find("Name")?.GetComponent<Text>();
        AssertNotNull(nameText, "nameText");

        weightText = transform.Find("Weight")?.GetComponent<Text>();
        AssertNotNull(weightText, "weightText");

        var classificationTransform = transform.Find("Classification");
        classificationText = classificationTransform?.Find("Value")?.GetComponent<Text>();
        AssertNotNull(classificationText, "classificationText");

        stabilityText = transform.Find("TextStability").GetComponent<Text>();
        AssertNotNull(stabilityText, "stabilityText");
        chargeText = transform.Find("TextCharge").GetComponent<Text>();
        AssertNotNull(chargeText, "chargeText");
    }
    private void AssertNotNull<T>(T obj, string propertyName)
    {
        var stackTrace = new StackTrace();
        var callerName = stackTrace.GetFrame(1).GetMethod().Name;
        if (obj == null)
            throw new NullReferenceException($"Expected {propertyName} in call to panelName.{callerName}, got null");
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

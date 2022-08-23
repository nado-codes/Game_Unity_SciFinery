using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelProperties : MonoBehaviour
{
    private static PanelProperties instance;
    public static PanelProperties Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PanelProperties>();
            if (instance == null)
                throw new ApplicationException("Expected an instance of PanelProperties, found nothing");
            return instance;
        }
    }
    Text bondText, weightText, conductivityText, reactivityText, toxicityTest, meltingBoilingText;
    Text brittlenessText, malleabilityText;

    public void VerifyInitialize()
    {
        var instanceTransform = Instance.transform;

        bondText = instanceTransform.Find("Bond")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(bondText, "bondText");
        weightText = instanceTransform.Find("Weight")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(weightText, "weightText");
        conductivityText = instanceTransform.Find("Conductivity")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(conductivityText, "conductivityTest");
        reactivityText = instanceTransform.Find("Reactivity")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(reactivityText, "reactivityTest");
        toxicityTest = instanceTransform.Find("Toxicity")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(toxicityTest, "toxicityTest");
        meltingBoilingText = instanceTransform.Find("MeltingBoiling")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(meltingBoilingText, "meltingBoilingText");
        brittlenessText = instanceTransform.Find("Brittleness")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(brittlenessText, "brittlenessText");
        malleabilityText = instanceTransform.Find("Malleability")?.Find("Value")?.GetComponent<Text>();
        Assertions.AssertNotNull(malleabilityText, "malleabilityText");
    }
    void Start() => VerifyInitialize();

    public void Update()
    {

    }

    public static void SetElementData(Element element)
    {
        Instance.VerifyInitialize();

        if (element == null)
            throw new NullReferenceException("Expected an element in call to SetElementData, got undefined");

        var weight = element.Children.Select(c => c.Weight).Aggregate((w, a) => a + w);
        Instance.weightText.text = weight.ToString();



    }
}
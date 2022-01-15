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
        var protonCount = FileSystem.ActiveAtom.ProtonCount;
        var neutronCount = FileSystem.ActiveAtom.NeutronCount;

        numberText.text = protonCount.ToString();
        weightText.text = FileSystem.ActiveAtom.Weight.ToString()+".00";

        NameText.text = FileSystem.ActiveAtom.Name;
        Editor.atomGameObject.name = "Atom"+FileSystem.ActiveAtom.Name;
        
        if(FileSystem.ActiveAtom.Charge == 0)
            shortNameText.text = FileSystem.ActiveAtom.ShortName;
        else if(FileSystem.ActiveAtom.Charge < 0)
            shortNameText.text = FileSystem.ActiveAtom.ShortName+"-";
        else if(FileSystem.ActiveAtom.Charge > 0)
            shortNameText.text = FileSystem.ActiveAtom.ShortName+"+";
    }
}

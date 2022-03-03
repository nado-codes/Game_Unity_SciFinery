using System;
using UnityEngine.UI;
using UnityEngine;

public class DialogYesNo : MonoBehaviour
{
    protected static DialogYesNo instance;
    private static Text txTitle, txBody;
    private static VoidFN _fnYes, _fnNo;

    protected void Start()
    {
        if(instance == null)
            instance = gameObject.GetComponent<DialogYesNo>();
        else
            throw new ApplicationException("There may only be one instance of type DialogYesNo, found at least 2");

        var header = instance.transform.Find("panelHeader");
        txTitle = header.transform.Find("Title").GetComponent<Text>();
        txBody = instance.transform.Find("Body").GetComponent<Text>();

        Close();
    }

    public static void Open(string title, string body, VoidFN fnYes, VoidFN fnNo = null) {

        _fnYes = fnYes;
        _fnNo = fnNo ?? (() => {});
        instance.gameObject.SetActive(true);
    }
    public static void DoYes()
    {
        _fnYes?.Invoke();
        Close();
    }

    public static void DoNo()
    {
        _fnNo?.Invoke();
        Close();
    }
    
    public static void Close() => instance.gameObject.SetActive(false);
}

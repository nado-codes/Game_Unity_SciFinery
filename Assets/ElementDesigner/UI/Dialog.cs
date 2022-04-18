using System;
using UnityEngine.UI;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    protected static Dialog instance;
    private Text txTitle, txBody;

    private void verifyInitialize()
    {
        if (instance == null)
            instance = gameObject.GetComponent<Dialog>();

        var header = instance.transform.Find("panelHeader");
        txTitle = header.transform.Find("Title").GetComponent<Text>();
        txBody = instance.transform.Find("Body").GetComponent<Text>();
    }
    protected void Start()
    {
        verifyInitialize();
        Close();
    }

    public virtual void Open(string title, string body)
    {
        verifyInitialize();
        txTitle.text = title;
        txBody.text = body;

        instance.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        verifyInitialize();
        instance.gameObject.SetActive(false);
    }
}

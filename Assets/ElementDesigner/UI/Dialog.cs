using System;
using UnityEngine.UI;
using UnityEngine;

public delegate T tFN<T>();

public class Dialog : MonoBehaviour
{
    protected static Dialog instance;
    private static Text txTitle, txBody;

    protected void Start()
    {
        if (instance == null)
            instance = gameObject.GetComponent<Dialog>();

        var header = instance.transform.Find("panelHeader");
        txTitle = header.transform.Find("Title").GetComponent<Text>();
        txBody = instance.transform.Find("Body").GetComponent<Text>();

        Close();
    }

    public virtual void Open(string title, string body)
    {

        txTitle.text = title;
        txBody.text = body;

        instance.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        instance.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;

public class DialogYesNo : MonoBehaviour
{
    protected static DialogYesNo instance;

    protected void Start()
    {
        instance = gameObject.GetComponent<DialogYesNo>();
        Close();
    }

    public static void Open() => instance.gameObject.SetActive(true);
    public static void Close() => instance.gameObject.SetActive(false);
}

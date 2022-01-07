using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogEditName : MonoBehaviour
{
    InputField inputName;
    
    // Start is called before the first frame update
    void Start()
    {
        inputName = transform.Find("inputName").GetComponent<InputField>();
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            Accept();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        inputName.text = panelName.NameText.text;
        HUD.LockedFocus = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    public void Accept()
    {
        panelName.NameText.text = inputName.text;
        Close();
    }
}

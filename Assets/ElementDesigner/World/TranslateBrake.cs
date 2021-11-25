using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateBrake : Interact
{
    private Translate parent;
    private Color rendererOriginColor;
    public bool IsActive = false;

    private DateTime lastClick;
    new void Start()
    {
        parent = GetComponentInParent<Translate>();
        rendererOriginColor = GetComponent<Renderer>().material.color;
        base.Start();
    }

    new void Update()
    {
        if(Input.GetMouseButton(0) && isHovered)
            GetComponent<Renderer>().material.color = new Color(1,0,0,.5f);
        else
            GetComponent<Renderer>().material.color = rendererOriginColor;
        
        base.Update();
    }

    private new void OnMouseDown()
    {
        var currentTime = DateTime.Now;

        if((currentTime-lastClick).Milliseconds < 500)
            parent.AllStop();

        lastClick = DateTime.Now;
        parent.SetTranslateIsActive(true);
        IsActive = true;
        base.OnMouseDown();
    }

    public new void Select()
    {
        base.Select();
        highlightCubeRenderer.material.color = new Color(1,0,0,.5f);
    }
    public 
    void OnMouseUp()
    {
        parent.SetTranslateIsActive(false);
        IsActive = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateHandle : Interact
{
    private Translate parent;
    public Vector3 startPosition {get; private set;}
    public bool IsActive = false;
    
    new void Start()
    {
        parent = GetComponentInParent<Translate>();
        startPosition = transform.position;
        base.Start();
    }

    private new void OnMouseDown()
    {
        parent.SetTranslateIsActive(true);
        IsActive = true;
        startPosition = transform.position;
        base.OnMouseDown();
    }

    void OnMouseUp()
    {
        parent.SetTranslateIsActive(false);
        IsActive = false;
        transform.position = startPosition;
    }
}

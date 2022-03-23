using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void ElementDataDelegate<T>(T elementData) where T : Element;
public class ElementGridItem<T> : GridItem, IPointerDownHandler where T : Element
{
    public T elementData;
    public bool hasData = false; // .. Because of the way Unity works, we can't use "elementData == null" to test if data exists. Use a bool.

    public ElementDataDelegate<T> OnClick;

    protected override void VerifyInitialize()
    {
        base.VerifyInitialize();

        nameText.text = elementData?.Name ?? string.Empty;
    }

    public void OnPointerDown(PointerEventData ev) => HandleClick();

    public void HandleClick() => OnClick?.Invoke(elementData);

    public virtual void SetData(T data)
    {
        elementData = data;
        hasData = data != null;
    }


}

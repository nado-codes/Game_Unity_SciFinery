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

    public ElementDataDelegate<T> OnClick;

    protected override void VerifyInitialize()
    {
        base.VerifyInitialize();

        nameText.text = elementData?.Name ?? string.Empty;
    }

    public void OnPointerDown(PointerEventData ev) => HandleClick();

    public void HandleClick() => OnClick?.Invoke(elementData);


}

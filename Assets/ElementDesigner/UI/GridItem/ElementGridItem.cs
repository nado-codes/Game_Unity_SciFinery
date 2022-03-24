using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void ElementDataDelegate<T>(T elementData) where T : Element;
public class ElementGridItem<T> : GridItem, IPointerDownHandler where T : Element
{
    public T elementData;
    public bool hasData = false; // .. Because of the way Unity works, we can't use "elementData == null"
    public ElementDataDelegate<T> OnClick;

    protected Transform ActiveLayout;


    protected override void Start() => SetActive(hasData);

    protected override void VerifyInitialize()
    {
        base.VerifyInitialize();

        ActiveLayout = transform.Find("Layout_Std");

        nameText.text = elementData?.Name ?? string.Empty;
    }

    public void OnPointerDown(PointerEventData ev) => HandleClick();

    public void HandleClick() => OnClick?.Invoke(elementData);

    public void UpdateLayout()
    {
    }

    public virtual void SetData(T data)
    {
        elementData = data;
        hasData = data != null;

        // Update layout
        Transform layoutToUse = transform.Find($"Layout_{typeof(T).FullName}") ?? transform.Find("Layout_Std");
        IEnumerable<Transform> allLayouts = GetComponentsInChildren<Transform>().Where(t => t.name.Contains("Layout"));
        List<Transform> otherLayouts = allLayouts.Where(l => l != layoutToUse).ToList();

        otherLayouts.ForEach(l => l.gameObject.SetActive(false));
        layoutToUse.gameObject.SetActive(true);
        ActiveLayout = layoutToUse;
    }


}

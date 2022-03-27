using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void ElementDataDelegate(Element elementData);
public class ElementGridItem : MonoBehaviour, IPointerDownHandler
{
    public bool hasData = false; // .. Because of the way Unity works, we can't use "elementData == null"
    public ElementDataDelegate OnClick;
    public Element elementData;
    public ElementType elementDataType = ElementType.None;
    protected Text numberText, nameText, shortNameText, weightText;
    protected Transform ActiveLayout;
    protected Button button;
    protected bool initialized = false;
    private ColorBlock buttonColorsActive, buttonColorsInactive;

    public void OnPointerDown(PointerEventData ev) => HandleClick();
    public void HandleClick() => OnClick?.Invoke(elementData);
    public virtual void SetData(Element data)
    {
        elementData = data;
        hasData = data != null;
        elementDataType = Enum.Parse<ElementType>(data.GetType().FullName);

        UpdateLayout();
        SetActive(data != null);
    }
    public virtual void SetActive(bool active)
    {
        VerifyInitialize();

        List<Transform> allLayouts = GetComponentsInChildren<Transform>().Where(t => t.name.Contains("Layout")).ToList();
        allLayouts.ForEach(layout => layout.gameObject.SetActive(active));

        button.interactable = active;
    }
    protected virtual void VerifyInitialize()
    {
        if (initialized)
            return;

        initialized = true;

        button = GetComponent<Button>();
        buttonColorsActive = button.colors;
        buttonColorsInactive.normalColor = button.colors.disabledColor;
        buttonColorsInactive.highlightedColor = button.colors.disabledColor;
        buttonColorsInactive.pressedColor = button.colors.disabledColor;
        buttonColorsInactive.selectedColor = button.colors.disabledColor;

        UpdateLayout();
    }
    protected virtual void Start() => SetActive(hasData);

    private void UpdateLayout()
    {
        // Update layout
        Transform layoutToUse = transform.Find($"Layout_{elementDataType}") ?? transform.Find("Layout_Std");
        IEnumerable<Transform> allLayouts = GetComponentsInChildren<Transform>().Where(t => t.name.Contains("Layout"));
        List<Transform> otherLayouts = allLayouts.Where(l => l != layoutToUse).ToList();

        otherLayouts.ForEach(l => l.gameObject.SetActive(false));
        layoutToUse.gameObject.SetActive(true);
        ActiveLayout = layoutToUse;

        // Update UI
        nameText = ActiveLayout.Find("Name")?.GetComponent<Text>();
        nameText.text = elementData?.Name ?? string.Empty;
    }
}

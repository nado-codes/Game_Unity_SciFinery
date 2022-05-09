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
    public ElementType elementDataType = ElementType.Particle;
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

        Enum.TryParse(data?.GetType().FullName, out ElementType type);
        elementDataType = type;

        UpdateLayout();
        SetActive(data != null);
    }
    public virtual void SetActive(bool active)
    {
        VerifyInitialize();

        List<Transform> allLayouts = GetComponentsInChildren<Transform>(true).Where(t => t.name.Contains("Layout")).ToList();
        allLayouts.ForEach(layout =>
        {
            if (layout != ActiveLayout || !active)
                layout.gameObject.SetActive(false);
            else if (layout == ActiveLayout)
                ActiveLayout.gameObject.SetActive(true);
        });

        button.interactable = active;
    }

    // START
    protected virtual void Start() => SetActive(hasData);

    protected virtual void VerifyInitialize()
    {
        if (initialized)
            return;

        initialized = true;

        button = GetComponent<Button>();
        Assertions.AssertNotNull(button, "button");
        buttonColorsActive = button.colors;
        buttonColorsInactive.normalColor = button.colors.disabledColor;
        buttonColorsInactive.highlightedColor = button.colors.disabledColor;
        buttonColorsInactive.pressedColor = button.colors.disabledColor;
        buttonColorsInactive.selectedColor = button.colors.disabledColor;

        UpdateLayout();
    }

    private void UpdateLayout()
    {
        // Update layout
        var elementLayoutTransform = transform.Find("ElementLayout");
        IEnumerable<Transform> allLayouts = elementLayoutTransform.GetComponentsInChildren<Transform>().Where(t => t.name.Contains("Layout"));
        Transform elementLayout = allLayouts.FirstOrDefault(l => l.name == $"Layout_{elementDataType}") ?? transform.Find("Layout_Std");
        Transform layoutToUse = elementData != null ? elementLayout : transform.Find("Layout_Std");
        layoutToUse?.gameObject.SetActive(true);
        ActiveLayout = layoutToUse;
        Assertions.AssertNotNull(ActiveLayout, "ActiveLayout");

        List<Transform> otherLayouts = allLayouts.Where(l => l != layoutToUse).ToList();
        otherLayouts.ForEach(l => l.gameObject.SetActive(false));

        // Update UI
        nameText = ActiveLayout.Find("Name")?.GetComponent<Text>();
        Assertions.AssertNotNull(nameText, "nameText");
        nameText.text = elementData?.Name ?? string.Empty;
    }
}

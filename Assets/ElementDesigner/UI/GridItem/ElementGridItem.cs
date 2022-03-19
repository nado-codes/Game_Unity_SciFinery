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

    protected Text numberText, shortNameText, nameText, weightText;
    private ColorBlock buttonColorsActive, buttonColorsInactive;
    protected Button button;
    private bool isHovered;

    public void Init()
    {
        numberText = transform.Find("Number")?.GetComponent<Text>();
        shortNameText = transform.Find("ShortName")?.GetComponent<Text>();
        nameText = transform.Find("Name")?.GetComponent<Text>();
        weightText = transform.Find("Weight")?.GetComponent<Text>();

        nameText.text = elementData?.Name ?? string.Empty;

        button = GetComponent<Button>();
        buttonColorsActive = button.colors;

        buttonColorsInactive.normalColor = button.colors.disabledColor;
        buttonColorsInactive.highlightedColor = button.colors.disabledColor;
        buttonColorsInactive.pressedColor = button.colors.disabledColor;
        buttonColorsInactive.selectedColor = button.colors.disabledColor;

        SetActive(elementData != null);
    }
    protected virtual void Start() => Init();
    protected virtual void Awake() => Init();

    public void OnPointerDown(PointerEventData ev) => HandleClick();

    public void HandleClick() => OnClick?.Invoke(elementData);

    public virtual void SetActive(bool active)
    {
        var allText = transform.GetComponentsInChildren<Text>().ToList();
        allText.ForEach(t => t.gameObject.SetActive(active));
        button.interactable = active;
    }
}

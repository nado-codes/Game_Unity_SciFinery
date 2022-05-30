using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public VoidFN OnClick;
    private Image image;
    private Color normalColor, selectedColor, hoverColor, disabledColor;
    private bool hovered = false, selected = false;
    public bool Disabled = false;

    void Awake()
    {
        image = GetComponent<Image>();

        disabledColor = new Color(image.color.r, image.color.g, image.color.b, .25f);
        hoverColor = new Color(image.color.r, image.color.g, image.color.b, .75f);
        normalColor = new Color(image.color.r, image.color.g, image.color.b, .5f);
        selectedColor = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    void Update()
    {
        if (Disabled)
            image.color = disabledColor;
    }

    public virtual void OnPointerEnter(PointerEventData ev)
    {
        if (Disabled) return;

        image.color = (!hovered && !selected) ? hoverColor : image.color;
        hovered = true;
    }

    public virtual void OnPointerExit(PointerEventData ev)
    {
        if (Disabled) return;

        image.color = !selected ? normalColor : image.color;
        hovered = false;
    }

    public void OnPointerClick(PointerEventData ev)
    {
        if (Disabled) return;

        OnClick?.Invoke();
        selected = true;
    }

    public void Select()
    {
        selected = true;
        image.color = selectedColor;
    }
    public void Deselect()
    {
        selected = false;
        image.color = !hovered ? normalColor : hoverColor;
    }
}

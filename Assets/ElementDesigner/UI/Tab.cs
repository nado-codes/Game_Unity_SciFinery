using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public VoidFN OnClick;
    private Image image;
    private Color normalColor, selectedColor, hoverColor;
    private bool hovered = false, selected = false;

    void Awake()
    {
        image = GetComponent<Image>();

        hoverColor = new Color(image.color.r, image.color.g, image.color.b, .75f);
        normalColor = new Color(image.color.r, image.color.g, image.color.b, .5f);
        selectedColor = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    public void OnPointerEnter(PointerEventData ev)
    {
        image.color = (!hovered && !selected) ? hoverColor : image.color;
        hovered = true;
    }

    public void OnPointerExit(PointerEventData ev)
    {
        image.color = !selected ? normalColor : image.color;
        hovered = false;
    }

    public void OnPointerClick(PointerEventData ev)
    {
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

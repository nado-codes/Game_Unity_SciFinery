using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ElementDataDelegate<T>(T elementData) where T : Element;
public class GridItem<T> : MonoBehaviour where T : Element
{
    public T elementData = null;
    protected Button button;

    public ElementDataDelegate<T> OnClick;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        SetActive(false);
    }
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        SetActive(false);
    }

    private void HandleClick() => OnClick?.Invoke(elementData);

    public virtual void SetActive(bool active)
    {
        button.interactable = active;
    }


}

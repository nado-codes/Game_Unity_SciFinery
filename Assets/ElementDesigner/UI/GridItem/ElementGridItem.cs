using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementGridItem : MonoBehaviour
{
    public Atom atom = null;
    protected Button button;

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

    public virtual void SetActive(bool active)
    {
        button.interactable = active;
    }
}

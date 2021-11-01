using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : MonoBehaviour
{
    private static Transform _selection;
    public static Transform Selection => _selection;
    private static Transform hover;
    
    private static Transform widgetTransform;

    // Start is called before the first frame update
    void Start()
    {
        widgetTransform = transform;
        gameObject.SetActive(false);
    }

    public static void Hover(Transform transform)
    {
        hover = transform;
    }

    public static void Select(Transform transform)
    {
        _selection = transform;
        widgetTransform.parent = _selection;
        widgetTransform.localPosition = Vector3.zero;

        widgetTransform.gameObject.SetActive(true);
    }

    public static void Deselect()
    {
        Debug.Log("cleared translate");
        widgetTransform.parent = null;
        widgetTransform.gameObject.SetActive(false);
    }
}

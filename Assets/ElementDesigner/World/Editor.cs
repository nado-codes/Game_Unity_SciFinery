using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private static List<Interact> selection = new List<Interact>();
    public static Interact LastHovered;

    private GameObject dragBox;
    private Vector3 dragBoxStart, dragBoxEnd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetMouseButtonUp(0))
            Select(LastHovered);

        /* if(Input.GetMouseButtonDown(0))
            StartDragSelect(); */

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            selection.ForEach(s => GameObject.Destroy(s.gameObject));
            selection.Clear();
        }
    }

    /* void StartDragSelect()
    {
        dragBox = GameObject.CreatePrimitive(PrimitiveType.Plane);
        dragBox.AddComponent<FaceCam>();

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragBoxStart = cameraRay.origin;
        Debug.Log("start drag select");
    }

    void StopDragSelect()
    {
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragBoxEnd = cameraRay.origin;

        GameObject.Destroy(dragBox);
        Debug.Log("stop drag select");
    } */

    public static void SetHover(Interact objectToHover)
    {
        LastHovered = objectToHover;
        Debug.Log("hovering "+objectToHover.gameObject.name);
    }

    public static void ClearHover()
    {
        LastHovered = null;
    }

    public static void Select(Interact objectToSelect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        if(!isMultiSelect && !isMultiDeselect)
        {
            Debug.Log("cleared other selection");
            var objectsToDeselect = selection.Where(s => s != objectToSelect).ToList();
            objectsToDeselect.ForEach(s => s.Deselect());
            selection.RemoveAll(s => objectsToDeselect.Contains(s));
        }

        if(!objectToSelect) return;

        if(!isMultiDeselect)
        {
            if(!selection.Contains(objectToSelect))
            {
                Debug.Log("added "+objectToSelect.gameObject.name+" to selection");
                selection.Add(objectToSelect);
            }
        }
        else
        {
            if(selection.Contains(objectToSelect))
                Debug.Log("removed "+objectToSelect.gameObject.name+" from selection");
            
            objectToSelect.Deselect();
            selection.Remove(objectToSelect);
        }
    }

    public static void Deselect(Interact objectToDeselect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);

        if(!isMultiSelect)
        {
            Debug.Log("removed "+objectToDeselect.gameObject.name+" from selection");
            objectToDeselect.Deselect();
            selection.Remove(objectToDeselect);
        }
    }
}

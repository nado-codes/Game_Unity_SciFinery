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
    private Vector2 mouseStart,mouseEnd;

    // Start is called before the first frame update
    void Start()
    {
        // BeginDragSelect();
    }

    // Update is called once per frame
    void Update()
    {   
        //if(Input.GetMouseButtonDown(0))
          //  BeginDragSelect();

        if(Input.GetMouseButton(0))
        {
            /*var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            dragBoxEnd = cameraRay.origin;
            mouseEnd = Input.mousePosition;

            //TODO: update position of drag box
            var dist = new Vector3(
                Mathf.Abs(dragBoxStart.x-dragBoxEnd.x),
                Mathf.Abs(dragBoxStart.y-dragBoxEnd.y),
                Mathf.Abs(dragBoxStart.z-dragBoxEnd.z)
            );

            var mouseDist = new Vector2(
                Mathf.Abs(mouseStart.x-mouseEnd.x),
                Mathf.Abs(mouseStart.y-mouseEnd.y)
            );

            Debug.Log("dist="+dist);
            Debug.Log("mouseDist="+mouseDist);

            // currentParticle.transform.position = cameraRay.origin + (cameraRay.direction * particleDistance);

            dragBox.transform.localScale = new Vector3(1,0,1);
            dragBox.transform.position = dragBoxEnd + (cameraRay.direction * 2); */
        }

        if(Input.GetMouseButtonUp(0))
        {
            Select(LastHovered);
            //FinishDragSelect();
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            selection.ForEach(s => GameObject.Destroy(s.gameObject));
            selection.Clear();
        }
    }

    void BeginDragSelect()
    {
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        mouseStart = Input.mousePosition;
        dragBoxStart = cameraRay.origin;
        Debug.Log("start drag select");

        Material dragboxMat = Resources.Load("ElementDesigner/DragboxMat.mat", typeof(Material)) as Material;
        dragBox = GameObject.CreatePrimitive(PrimitiveType.Plane);

        var dragboxRenderer = dragBox.GetComponent<Renderer>();
        dragboxRenderer.material.color = Color.blue;
        dragBox.transform.parent = Camera.main.transform;
        dragBox.transform.localPosition = dragBoxStart + Vector3.forward;
        dragBox.transform.localRotation = Quaternion.Euler(270,0,0);

        
    }

    void FinishDragSelect()
    {
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragBoxEnd = cameraRay.origin;

        GameObject.Destroy(dragBox);
        Debug.Log("stop drag select");
    }

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

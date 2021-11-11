using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private static List<Interact> selection = new List<Interact>();
    private static List<Interact> tempSelection = new List<Interact>();
    public static Interact LastHovered;

    public RectTransform selectionBoxRect;
    private BoxCollider dragSelectCollider;
    private Vector2 dragSelectStartPosition, endDragSelectPosition;
    private Ray dragSelectStartWorld, dragSelectEndWorld;

    public enum DragState{Init,Active,None}
    private DragState dragState = DragState.None;

    private Interact[] interactables;

    // Start is called before the first frame update
    void Start()
    {
        dragSelectCollider = gameObject.AddComponent<BoxCollider>();
        dragSelectCollider.enabled = false;

        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {  
        HandleDragSelect();

        if(Input.GetMouseButtonUp(0))
            Select(LastHovered);

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            selection.ForEach(s => GameObject.Destroy(s.gameObject));
            selection.Clear();
        }
    }

    void HandleDragSelect()
    {
        if(Input.GetMouseButtonDown(0))
            InitDragSelect();

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectEndWorld = cameraRay;

        if(Vector3.Distance(dragSelectStartWorld.origin,dragSelectEndWorld.origin) > .05f && dragState == DragState.Init)
            StartDragSelect();

        if(Input.GetMouseButton(0) && dragState == DragState.Active)
        {
            

            // .. collider
            var hudRectTransform = GameObject.Find("HUD").GetComponent<RectTransform>();

            var dragOffset = ((hudRectTransform.position-transform.position)*.9f).magnitude;
            var dragMin = dragSelectStartWorld.origin + (dragSelectStartWorld.direction*dragOffset);
            var dragMinLocal = transform.InverseTransformPoint(dragMin);
            var dragMax = dragSelectEndWorld.origin + (dragSelectEndWorld.direction*dragOffset);;
            var dragMaxLocal = transform.InverseTransformPoint(dragMax);
            var dragCenter = dragMin+(dragMax-dragMin)*.5f;
            var dragCenterLocal = transform.InverseTransformPoint(dragCenter);

            Debug.DrawRay(dragMin,transform.forward,Color.green,.01f);
            Debug.DrawRay(dragMax,transform.forward,Color.red,.01f);
            Debug.DrawRay(dragCenter,transform.forward,Color.blue,.01f);

            var colliderSizeBase = (dragMaxLocal-dragMinLocal);
            dragSelectCollider.size = new Vector3(colliderSizeBase.x*2,colliderSizeBase.y*2,20);
            dragSelectCollider.center = dragCenterLocal + (Vector3.forward*dragSelectCollider.size.z*.5f);

            // .. HUD selection box 
            float selectionBoxWidth = Input.mousePosition.x - dragSelectStartPosition.x;
            float selectionBoxHeight = Input.mousePosition.y - dragSelectStartPosition.y;
            selectionBoxRect.anchoredPosition = dragSelectStartPosition - new Vector2(-selectionBoxWidth / 2, -selectionBoxHeight / 2);
            selectionBoxRect.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth),Mathf.Abs(selectionBoxHeight));
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(dragState == DragState.Active)
                FinishDragSelect();

            dragState = DragState.None;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();
    
        if(interact && !tempSelection.Contains(interact))
        {
            interact.Hover();
            tempSelection.Add(interact);
        }
    }

    void OnTriggerExit(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();

        if(interact)
        {
            interact.ClearHover();
            tempSelection.Remove(interact);
        }
    }

    void InitDragSelect()
    {
        dragState = DragState.Init;
        dragSelectStartWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectStartPosition = Input.mousePosition;
        Debug.Log("init drag select");
    }

    void StartDragSelect()
    {
        selectionBoxRect.gameObject.SetActive(true);
        selectionBoxRect.sizeDelta = Vector2.zero;
        dragSelectCollider.enabled = true;
        dragSelectCollider.isTrigger = true;

        dragState = DragState.Active;
        Debug.Log("start drag select");
    }

    void FinishDragSelect()
    {
        endDragSelectPosition = Input.mousePosition;
        selectionBoxRect.gameObject.SetActive(false);
        dragSelectCollider.enabled = false;
        dragSelectCollider.isTrigger = false;
        SelectMany(tempSelection);

        Debug.Log("finish drag select");
        
        
    }

    public static void SetHover(Interact objectToHover)
    {
        LastHovered = objectToHover;
        // Debug.Log("hovering "+objectToHover.gameObject.name);
    }

    public static void ClearHover()
    {
        LastHovered = null;
    }

    public static void SelectMany(IEnumerable<Interact> objectsToSelect)
    {
        Debug.Log("try to SelectMany on "+objectsToSelect.Count()+" objects");
        objectsToSelect.ToList().ForEach(s => s.Select());
    }

    public static void Select(Interact objectToSelect)
    {
        Debug.Log("selecting "+objectToSelect);

        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        Debug.Log("there are "+selection.Count+" objects currently selected");        

        if(!isMultiSelect && !isMultiDeselect)
        {
            
            var objectsToDeselect = selection.Where(s => s != objectToSelect && !tempSelection.Contains(s)).ToList();
            if(objectsToDeselect.Count > 0) Debug.Log("cleared "+objectsToDeselect.Count+" other objects from selection");
            objectsToDeselect.ForEach(s => s.Deselect());
            selection.RemoveAll(s => objectsToDeselect.Contains(s));
            
        }

        LastHovered = null;

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

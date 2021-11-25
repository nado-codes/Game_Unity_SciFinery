using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private static List<Interact> _selectedObjects = new List<Interact>();
    public static IEnumerable<Interact> SelectedObjects => _selectedObjects;
    private static List<Interact> hoveredObjects = new List<Interact>();

    public enum DragState{Init,Active,None}
    private static bool dragSelectIsEnabled = true;
    private DragState dragState = DragState.None;
    public RectTransform selectionBoxRect;
    private BoxCollider dragSelectCollider;
    private Vector2 dragSelectStartPosition, endDragSelectPosition;
    private Ray dragSelectStartWorld, dragSelectEndWorld;

    void Start()
    {
        dragSelectCollider = gameObject.AddComponent<BoxCollider>();
        dragSelectCollider.enabled = false;

        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    void Update()
    {  
        if(dragSelectIsEnabled)
            HandleDragSelect();

        if(Input.GetMouseButtonUp(0))
        {
            Select(hoveredObjects.Where(sel => sel.Selectable));
            hoveredObjects.Clear();
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            _selectedObjects.ForEach(s => {
                GameObject.Destroy(s.gameObject);
                World.RemoveParticle(s.GetComponent<Particle>());
            });
            _selectedObjects.Clear();
            
        }

        if(_selectedObjects.Any())
        {
            var selectionCenter = _selectedObjects.Count() > 1 ? 
                _selectedObjects.Aggregate(Vector3.zero,(total,next) => total += next.transform.position*.5f) :
                _selectedObjects.FirstOrDefault().transform.position;
        
            Translate.Instance.transform.position = selectionCenter;
        }
    }

    public static void SetDragSelectEnabled(bool enable) => dragSelectIsEnabled = enable;

    void HandleDragSelect()
    {
        if(Input.GetMouseButtonDown(0))
            InitDragSelect();

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectEndWorld = cameraRay;

        bool canDrag = (
            Vector3.Distance(dragSelectStartWorld.origin,dragSelectEndWorld.origin) > .05f &&
            !Translate.IsActive
        );

        if(canDrag && dragState == DragState.Init)
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
    
        if(interact && interact.Selectable && !hoveredObjects.Contains(interact))
        {
            interact.Hover();

            Debug.Log("adding "+interact.gameObject.name+" to selection");
            hoveredObjects.Add(interact);
        }
    }

    void OnTriggerExit(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();

        if(interact)
        {
            interact.ClearHover();
            hoveredObjects.Remove(interact);
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
    }

    public static void Hover(Interact objectToHover)
    {
        if(!hoveredObjects.Contains(objectToHover))
            hoveredObjects.Add(objectToHover);
    }

    public static void RemoveHover(Interact objectToDehover)
    {
        hoveredObjects.Remove(objectToDehover);
    }

    public static void Select(Interact objectToSelect) => Select(new Interact[1]{objectToSelect});

    public static void Select(IEnumerable<Interact> objectsToSelect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        // Debug.Log("there are "+selectedObjects.Count+" objects currently selected");
        Debug.Log("..."+objectsToSelect.Count()+" more objects will be selected/deselected");        

        if(!isMultiSelect && !isMultiDeselect)
        {
            var objectsToDeselect = _selectedObjects.Where(s => !objectsToSelect.Contains(s)).ToList();
            // if(objectsToDeselect.Count > 0) Debug.Log("cleared "+objectsToDeselect.Count+" other objects from selection");
            objectsToDeselect.ForEach(s => s.Deselect());
            _selectedObjects.RemoveAll(s => objectsToDeselect.Contains(s));
        }

        Translate.SetActive((objectsToSelect.Count() > 0));

        if(objectsToSelect.Count() == 0) return;

        if(!isMultiDeselect)
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Select());

            _selectedObjects.AddRange(objectsToSelect.Where(objectToSelect => {
                if(!_selectedObjects.Contains(objectToSelect))
                {
                    Debug.Log("trying to add"+objectToSelect);
                    Debug.Log("added "+objectToSelect.gameObject.name+" to selection");
                    return true;
                }
                return false;
            }));
        }
        else
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Deselect());

            objectsToSelect.ToList().ForEach(objectToDeselect => {
                if(_selectedObjects.Contains(objectToDeselect))
                    Debug.Log("removed "+objectToDeselect.gameObject.name+" from selection");

                objectToDeselect.Deselect();
                _selectedObjects.Remove(objectToDeselect);
            });
        }
    }

    public static void Deselect(Interact objectToDeselect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);

        if(!isMultiSelect)
        {
            Debug.Log("removed "+objectToDeselect.gameObject.name+" from selection");
            objectToDeselect.Deselect();
            _selectedObjects.Remove(objectToDeselect);
        }
    }
}

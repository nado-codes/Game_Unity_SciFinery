using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum CreationState { None, Start, Drag }

public class panelCreate : MonoBehaviour, IPointerExitHandler
{
    private static panelCreate instance;
    public static panelCreate Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<panelCreate>();
                instance?.VerifyInitialize();
            }

            return instance;
        }
    }

    public CreationState creationState = CreationState.None;
    private Element elementToCreateData;
    public WorldElement currentWorldElement;

    private List<Element> loadedElements = new List<Element>();
    private List<Element> visibleLoadedElements = new List<Element>();
    private Transform elementButtonsTransform, btnPrevTransform, btnNextTransform;
    private List<GridItem> elementButtons = new List<GridItem>();

    public float particleDefaultDistance = 5;
    private bool isHover = false;
    private float particleDistance = 1, zoomSensitivity = 400;

    private void VerifyInitialize()
    {
        if (instance != null)
            return;

        particleDistance = particleDefaultDistance;

        btnPrevTransform = transform.Find("btnPrev");
        btnNextTransform = transform.Find("btnNext");

        elementButtonsTransform = transform.Find("elementButtons");
        elementButtons = elementButtonsTransform.GetComponentsInChildren<GridItem>().ToList();
        // elementButtons.ForEach(btn => btn.UseGridItemForType(Editor.DesignType).OnClick += (data) => HandleElementGridItemClicked(data));

        instance = this;
    }

    void Start() => VerifyInitialize();



    public static void SetDesignType(ElementType newDesignType)
    {
        var subElements = loadSubElementsForDesignType(newDesignType);
        Instance.LoadElements(subElements);
    }

    private static IEnumerable<Element> loadSubElementsForDesignType(ElementType designType)
    => designType switch
    {
        ElementType.Atom => FileSystemCache.GetOrLoadSubElementsOfType(ElementType.Particle),
        ElementType.Molecule => FileSystemCache.GetOrLoadSubElementsOfType(ElementType.Atom),
        _ => throw new NotImplementedException($"Designs for elements of type \"{designType}\" is not yet implemented")
    };

    public void HandlePrevButtonClicked()
    {
        var firstVisibleElementIndex = loadedElements.IndexOf(visibleLoadedElements.FirstOrDefault());

        // .. NOTE: If we're at the start of the list, loop back to the end and add that one
        var elementToAdd = firstVisibleElementIndex > 0 ? loadedElements[firstVisibleElementIndex - 1] : loadedElements.LastOrDefault();
        visibleLoadedElements.Insert(0, elementToAdd);
        visibleLoadedElements.Remove(visibleLoadedElements.LastOrDefault());

        RenderVisibleElements();
    }

    public void HandleNextButtonClicked()
    {
        var lastVisibleElementIndex = loadedElements.IndexOf(visibleLoadedElements.LastOrDefault());

        // .. NOTE: If we're at the end of the list, loop back to the start and add that one
        var elementToAdd = lastVisibleElementIndex < loadedElements.Count - 1 ? loadedElements[lastVisibleElementIndex + 1] : loadedElements.FirstOrDefault();
        visibleLoadedElements.Add(elementToAdd);
        visibleLoadedElements.Remove(visibleLoadedElements.FirstOrDefault());

        RenderVisibleElements();
    }

    // particleToCreateData will specify all of the element's properties e.g. for molecules, which atoms it contains
    // the "elementType" passed into Editor.CreateWorldElement will determine how the incoming data is read
    public void OnPointerExit(PointerEventData ev)
    {
        if (creationState == CreationState.Start)
        {
            currentWorldElement = Editor.CreateSubElement(elementToCreateData);
            currentWorldElement.enabled = false;
            creationState = CreationState.Drag;

            Editor.SetDragSelectEnabled(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (creationState == CreationState.Drag)
        {
            // zoom
            float scroll = Input.mouseScrollDelta.y;

            bool isZoomIn = scroll < 0;
            bool isZoomOut = scroll > 0;

            if (isZoomOut && particleDistance < 50)
                particleDistance += zoomSensitivity * Time.deltaTime;

            if (isZoomIn && particleDistance > 5)
                particleDistance -= zoomSensitivity * Time.deltaTime;

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            currentWorldElement.transform.position = cameraRay.origin + (cameraRay.direction * particleDistance);

            if (Input.GetMouseButtonUp(0))
            {
                particleDistance = particleDefaultDistance;
                currentWorldElement.enabled = true;
                currentWorldElement = null;
                creationState = CreationState.None;
                Editor.SetDragSelectEnabled(true);
            }
        }
    }

    private void RenderVisibleElements()
    {
        foreach (Element element in visibleLoadedElements)
        {
            var elementIndex = visibleLoadedElements.IndexOf(element);
            GridItem gridItem = elementButtons[elementIndex];

            if (gridItem == null)
                throw new NullReferenceException($"Expected a gridItem at index {elementIndex} in call to panelCreate.RenderVisibleElements, got null");

            gridItem.SetData(element);
            gridItem.GetGridItemForType(element.ElementType).OnClick = HandleElementGridItemClicked;
        }
    }

    private void LoadElements(IEnumerable<Element> elements)
    {
        loadedElements = elements.ToList();
        var numGridItems = elementButtons.Count;
        visibleLoadedElements = elements.Where((_, i) => i >= 0 && i < numGridItems).ToList();
        RenderVisibleElements();
    }

    void OnMouseEnter()
    {
        isHover = true;
    }

    private void HandleElementGridItemClicked(Element elementData)
    {
        elementToCreateData = elementData;
        creationState = CreationState.Start;
    }
}

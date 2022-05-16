using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public enum CreationState { None, Start, Drag }

public class PanelCreate : MonoBehaviour, IPointerExitHandler
{
    private static PanelCreate instance;
    public static PanelCreate Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PanelCreate>();
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
    private Button btnPrev, btnNext;
    public float particleDefaultDistance = 5;
    private float particleDistance = 1, zoomSensitivity = 400;

    private void VerifyInitialize()
    {
        if (instance != null)
            return;

        particleDistance = particleDefaultDistance;

        btnPrevTransform = transform.Find("btnPrev");
        btnPrev = btnPrevTransform?.GetComponent<Button>();
        Assertions.AssertNotNull(btnPrev, "btnPrev");
        btnNextTransform = transform.Find("btnNext");
        btnNext = btnNextTransform?.GetComponent<Button>();
        Assertions.AssertNotNull(btnNext, "btnNext");

        elementButtonsTransform = transform.Find("elementButtons");
        elementButtons = elementButtonsTransform?.GetComponentsInChildren<GridItem>().ToList();
        Assertions.AssertNotEmpty(elementButtons, "elementButtons");

        instance = this;
    }
    void Start() => VerifyInitialize();

    public static void SetDesignType(ElementType newDesignType)
    {
        var subElements = loadSubElementsForDesignType(newDesignType);
        Instance.LoadElements(subElements);
    }
    public void HandlePrevButtonClicked()
    {
        var firstVisibleElementIndex = loadedElements.IndexOf(visibleLoadedElements.FirstOrDefault());

        // .. NOTE: If we're at the start of the list, loop back to the end and add that one
        var elementToAdd = firstVisibleElementIndex > 0 ? loadedElements[firstVisibleElementIndex - 1] : loadedElements.LastOrDefault();
        visibleLoadedElements.Insert(0, elementToAdd);
        visibleLoadedElements.Remove(visibleLoadedElements.LastOrDefault());

        renderVisibleElements();
    }
    public void HandleNextButtonClicked()
    {
        var lastVisibleElementIndex = loadedElements.IndexOf(visibleLoadedElements.LastOrDefault());

        // .. NOTE: If we're at the end of the list, loop back to the start and add that one
        var elementToAdd = lastVisibleElementIndex < loadedElements.Count - 1 ? loadedElements[lastVisibleElementIndex + 1] : loadedElements.FirstOrDefault();
        visibleLoadedElements.Add(elementToAdd);
        visibleLoadedElements.Remove(visibleLoadedElements.FirstOrDefault());

        renderVisibleElements();
    }
    public void OnPointerExit(PointerEventData ev)
    {
        if (creationState == CreationState.Start)
        {
            currentWorldElement = Editor.CreateSubElement(elementToCreateData);
            Assertions.AssertNotNull(currentWorldElement, "currentWorldElement");
            var motor = currentWorldElement.GetComponent<WorldElementMotor>();
            if (motor != null)
                motor.enabled = false;

            creationState = CreationState.Drag;

            EditorSelect.SetDragSelectEnabled(false);
        }
    }
    void Update()
    {
        if (creationState == CreationState.Drag)
        {
            HUD.LockFocus();
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

                var motor = currentWorldElement.GetComponent<WorldElementMotor>();
                if (motor != null)
                {
                    motor.enabled = true;
                    motor.AddVelocity(Camera.main.GetComponent<FlyCam>().Velocity);
                }

                currentWorldElement = null;
                creationState = CreationState.None;
                EditorSelect.SetDragSelectEnabled(true);
                HUD.ClearFocus();
            }
        }
    }

    private void renderVisibleElements()
    {
        elementButtons.ForEach(eb =>
        {
            var gridItem = eb.GetComponent<GridItem>();
            var elementGridItems = (gridItem?.GetComponents<ElementGridItem>() ?? Array.Empty<ElementGridItem>()).ToList();
            elementGridItems.ForEach(egi =>
            {
                egi.OnClick = null;
                egi.SetData(null);
            });
            gridItem.SetActive(false);
        });

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
        renderVisibleElements();

        var enableScrollButtons = elements.Count() > elementButtons.Count;
        btnNext.interactable = enableScrollButtons;
        btnPrev.interactable = enableScrollButtons;
    }
    private void HandleElementGridItemClicked(Element elementData)
    {
        elementToCreateData = elementData;
        creationState = CreationState.Start;
    }
    private static IEnumerable<Element> loadSubElementsForDesignType(ElementType designType)
    => designType switch
    {
        ElementType.Atom => FileSystemCache.GetOrLoadSubElementsOfType(ElementType.Particle),
        ElementType.Molecule => FileSystemCache.GetOrLoadSubElementsOfType(ElementType.Atom),
        _ => throw new NotImplementedException($"Designs for elements of type \"{designType}\" is not yet implemented")
    };
}

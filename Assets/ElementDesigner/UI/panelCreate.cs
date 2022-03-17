using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum CreationState { None, Start, Drag }

public class panelCreate : MonoBehaviour, IPointerExitHandler
{

    public CreationState creationState = CreationState.None;
    private static ElementType designType = ElementType.Atom;
    public ElementType elementToCreate = ElementType.Particle;
    private Element elementToCreateData;
    public WorldElement currentWorldElement;

    private List<Element> loadedElements = new List<Element>();
    private static Transform particleButtonsTransform, elementButtonsTransform, btnPrevTransform, btnNextTransform;
    private static List<AtomGridItem> elementButtons = new List<AtomGridItem>();

    public float particleDefaultDistance = 5;
    private bool isHover = false;
    private float particleDistance = 1, zoomSensitivity = 400;

    void Start()
    {
        particleDistance = particleDefaultDistance;
        particleButtonsTransform = transform.Find("particleButtons");
        elementButtonsTransform = transform.Find("elementButtons");
        btnPrevTransform = transform.Find("btnPrev");
        btnNextTransform = transform.Find("btnNext");

        var particleButtons = particleButtonsTransform.GetComponentsInChildren<ParticleGridItem>().ToList();
        particleButtons.ForEach(b => b.OnClick += HandleElementGridItemClicked<Particle>);

    }

    public void LoadParticles() => loadedElements.Clear();
    public void LoadElements(IEnumerable<Element> elements)
    {
        loadedElements.Clear();
        loadedElements.AddRange(elements);
    }

    public static void SetDesignType(ElementType newDesignType)
    {
        if (newDesignType != ElementType.Atom)
        {
            particleButtonsTransform.gameObject.SetActive(false);
            elementButtonsTransform.gameObject.SetActive(true);
            btnPrevTransform.gameObject.SetActive(true);
            btnNextTransform.gameObject.SetActive(true);

            // TODO: when elements are loaded in, set their onClick listener to pass their Element data to the particleToCreateData
            if (newDesignType == ElementType.Molecule)
            {
                // get or load atoms into spawning grid

            }
            else if (newDesignType == ElementType.Product)
            {
                // get or load molecules into spawning grid
            }
        }
        else
        {
            particleButtonsTransform.gameObject.SetActive(true);
            elementButtonsTransform.gameObject.SetActive(false);
            btnPrevTransform.gameObject.SetActive(false);
            btnNextTransform.gameObject.SetActive(false);
        }

        designType = newDesignType;
    }

    public static void HandlePrevButtonClicked()
    {
        // TODO: scroll the view to the previous atom or molecule
    }

    public static void HandleNextButtonClicked()
    {
        // TODO: scroll to the next atom or molecule
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

            if (isZoomIn)
                Debug.Log("zoom in");

            if (isZoomOut)
                Debug.Log("zoom out");

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

    void OnMouseEnter()
    {
        isHover = true;
    }

    private void HandleElementGridItemClicked<T>(T elementData) where T : Element
    {
        elementToCreateData = elementData;
        creationState = CreationState.Start;
        Debug.Log("starting creation");
    }


    // TODO: spawn particle when mouse exits panel
    // TODO: drop particle when mouse is up and set state to NONE

    // TODO: when the pointer exits, spawn the particleToCreate and create it with particleToCreateData
    // particleToCreateData will specify all of the element's properties e.g. for molecules, which atoms it contains
    // the "elementType" passed into Editor.CreateWorldElement will determine how the incoming data is read
    public void OnPointerExit(PointerEventData ev)
    {
        if (creationState == CreationState.Start)
        {
            Debug.Log("start drag");

            currentWorldElement = Editor.CreateWorldElement(elementToCreateData);
            currentWorldElement.enabled = false;
            creationState = CreationState.Drag;

            Editor.SetDragSelectEnabled(false);
        }
    }


}

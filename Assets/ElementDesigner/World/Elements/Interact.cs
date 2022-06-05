using System;
using UnityEngine;

public class Interact : MonoBehaviour
{
    enum InteractionState { None, Highlight, Select }
    private bool initialized = false;

    public bool Selectable = true;

    private float transparency = .5f;
    private Color HighlightedColor = new Color(1, 1, 1, .5f);
    public Color SelectedColor = new Color(0, 1, 0, .5f);

    private GameObject highlightCube;
    protected Renderer highlightCubeRenderer
    {
        get => highlightCube?.GetComponent<Renderer>();
    }
    public bool isHovered = false;
    private bool isSelected = false;

    private GridItem elementDisplay;
    public GridItem ElementDisplay
    {
        get
        {
            if (elementDisplay == null)
            {
                var bodyTransform = transform.Find("Body");
                var rotorTransform = bodyTransform?.Find("Rotor");
                var infoCanvasTransform = rotorTransform?.Find("InfoCanvas");
                var elementLayoutTransform = infoCanvasTransform?.Find("ElementLayout");
                elementDisplay = elementLayoutTransform?.GetComponent<GridItem>();
                Assertions.AssertNotNull(elementDisplay, "ElementDisplay");
                elementDisplay.gameObject.SetActive(false);
            }
            return elementDisplay;
        }
    }

    private DateTime lastClickStart;

    private void VerifyInitialize()
    {
        if (initialized) return;

        Material tMat = Resources.Load("ElementDesigner/TransparentWhite.mat", typeof(Material)) as Material;

        highlightCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlightCube.GetComponent<Collider>().enabled = false;
        highlightCube.transform.parent = transform;
        highlightCube.transform.localPosition = Vector3.zero;

        highlightCubeRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        highlightCube.SetActive(false);

        highlightCube.transform.localScale = Vector3.one * 1.1f;

        initialized = true;
    }
    protected void Start()
    {
        VerifyInitialize();
    }
    public void Update()
    {
        SelectedColor.a = transparency;
        HighlightedColor.a = transparency;

        if (Input.GetMouseButtonUp(0) && !Selectable)
            highlightCubeRenderer.material.color = HighlightedColor;

        if (!Input.GetMouseButton(0) && !isHovered)
            highlightCube?.SetActive(false);

        if (Input.GetKeyDown(KeyCode.F) && isSelected)
            focusCamera();
    }

    // Hover behaviour
    public void Hover()
    {
        VerifyInitialize();
        if (!isSelected)
            highlightCubeRenderer.material.color = HighlightedColor;
        ElementDisplay.gameObject.SetActive(true);

        highlightCube?.SetActive(true);
        isHovered = true;
    }
    void OnMouseEnter()
    {
        if (Selectable)
            EditorSelect.Hover(this);

        Hover();
    }

    // Unhover behaviour
    public void ClearHover()
    {
        VerifyInitialize();
        if (!isSelected)
            highlightCube?.SetActive(false);

        ElementDisplay.gameObject.SetActive(false);
        isHovered = false;
    }
    void OnMouseExit()
    {
        EditorSelect.RemoveHover(this);

        if (!isSelected && Selectable)
            ClearHover();
    }

    // Select behaviour
    public void Select()
    {
        VerifyInitialize();
        highlightCubeRenderer.material.color = SelectedColor;
        isSelected = true;
    }
    public void Deselect()
    {
        VerifyInitialize();
        isSelected = false;
        highlightCube.SetActive(false);
        ElementDisplay.gameObject.SetActive(false);
    }
    protected void OnMouseDown()
    {
        if (Selectable)
            EditorSelect.Select(this);

        Select();
    }
    protected void OnMouseUp()
    {
        var nowTime = DateTime.Now;
        if ((nowTime - lastClickStart).Milliseconds < 200)
            onDoubleClick();

        lastClickStart = nowTime;
    }

    private void onDoubleClick()
    {
        focusCamera();
    }

    private void focusCamera()
    {
        var orbitCam = Camera.main.GetComponent<OrbitCam>();

        if (orbitCam != null)
            orbitCam.TrackedObject = transform;

        EditorSelect.SetDragSelectEnabled(false);
    }

}

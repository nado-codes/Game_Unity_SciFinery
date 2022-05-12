using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    enum InteractionState { None, Highlight, Select }

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

    protected void Start()
    {
        Material tMat = Resources.Load("ElementDesigner/TransparentWhite.mat", typeof(Material)) as Material;

        highlightCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlightCube.GetComponent<Collider>().enabled = false;
        highlightCube.transform.parent = transform;
        highlightCube.transform.localPosition = Vector3.zero;

        highlightCubeRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        highlightCube.SetActive(false);

        highlightCube.transform.localScale = Vector3.one * 1.1f;
    }
    public void Update()
    {
        SelectedColor.a = transparency;
        HighlightedColor.a = transparency;

        if (Input.GetMouseButtonUp(0) && !Selectable)
            highlightCubeRenderer.material.color = HighlightedColor;

        if (!Input.GetMouseButton(0) && !isHovered)
            highlightCube?.SetActive(false);
    }

    // Hover behaviour
    public void Hover()
    {
        if (!isSelected)
            highlightCubeRenderer.material.color = HighlightedColor;

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
        if (!isSelected)
            highlightCube?.SetActive(false);

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
        highlightCubeRenderer.material.color = SelectedColor;
        isSelected = true;
    }
    public void Deselect()
    {
        isSelected = false;
        highlightCube.SetActive(false);
    }
    protected void OnMouseDown()
    {
        if (Selectable)
            EditorSelect.Select(this);

        Select();
    }


}

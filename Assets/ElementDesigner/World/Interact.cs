using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    enum InteractionState {None, Highlight, Select}

    public bool Selectable = true;

    private Color HighlightedColor = new Color(1,1,1,.5f);
    private Color SelectedColor = new Color(0,1,0,.5f);

    private InteractionState interactionState = InteractionState.None;
    private GameObject highlightCube;
    private Renderer highlightCubeRenderer {
        get => highlightCube?.GetComponent<Renderer>(); 
    }
    private bool isHovered = false;
    private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
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

    void Update()
    {
        if(Input.GetMouseButtonUp(0) && !Selectable)
            highlightCubeRenderer.material.color = HighlightedColor;
    }

    public void Deselect()
    {
        isSelected = false;
        highlightCube.SetActive(false);
    }

    // Hover behaviour
    void OnMouseEnter()
    {
        if(!isSelected)
            highlightCubeRenderer.material.color = HighlightedColor;

        highlightCube.SetActive(true);
        Editor.SetHover(this);
        isHovered = true;
    }

    // Unhover behaviour
    void OnMouseExit()
    {
        if(!isSelected)
            highlightCube.SetActive(false);

        Editor.ClearHover();
        isHovered = false;
    }

    // Select behaviour
    void OnMouseDown()
    {
        if(Selectable)
            Editor.Select(this);

        highlightCubeRenderer.material.color = SelectedColor;
        isSelected = true;
    }
}

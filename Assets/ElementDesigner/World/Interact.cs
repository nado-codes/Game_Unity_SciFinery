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

        highlightCubeRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));;
        highlightCube.SetActive(false);

        highlightCube.transform.localScale = Vector3.one * 1.1f;

        Debug.Log("interact has started");
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0) && !Selectable)
        {
            if(Editor.CurrentHighlight == this)
                highlightCubeRenderer.material.color = HighlightedColor;
            else
                highlightCube.SetActive(false);
        }
    }


    public void Deselect()
    {
        isSelected = false;

        if(Editor.CurrentHighlight != this)
            highlightCube.SetActive(false);
        else
            highlightCubeRenderer.material.color = HighlightedColor;
    }

    // Highlight behaviour
    void OnMouseEnter()
    {
        // Editor.Highlight(this);
        if(!isSelected)
        {
            highlightCubeRenderer.material.color = HighlightedColor;
            highlightCube.SetActive(true);

            // interactionState = InteractionState.Highlight;
        }

        // Editor.CurrentHighlight = this;
        // isHovered = true;
        
    }

    // De-highlight behaviour
    void OnMouseExit()
    {
        // Editor.Highlight(false);
        if(!isSelected)
        {
            highlightCube.SetActive(false);
            // interactionState = InteractionState.None;
        }

        // isHovered = false;
        Editor.CurrentHighlight = null;
    }

    // Select behaviour
    void OnMouseDown()
    {
        Editor.Select(this);
        highlightCubeRenderer.material.color = SelectedColor;
        isSelected = true;
        // isSelected = true;
        /* if(interactionState == InteractionState.Highlight)
        {
            Debug.Log("selected object");

            

            highlightCubeRenderer.material.color = SelectedColor;
            interactionState = InteractionState.Select;
        }*/
    }
}

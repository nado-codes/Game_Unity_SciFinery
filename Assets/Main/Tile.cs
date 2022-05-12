using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool selected = false;
    private Collider col;
    private Color hoverColor;
    private Color stdColor;
    private MeshRenderer mesh;
    private WorldGen worldGen;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshRenderer>();
        worldGen = GetComponentInParent<WorldGen>();

        stdColor = mesh.material.color;
        hoverColor = stdColor * 1.25f;
    }

    public void Deselect()
    {
        selected = false;
        ShowHover(false);
    }

    private void ShowHover(bool show) => mesh.material.color = !show ? stdColor : hoverColor;
    void OnMouseEnter()
    {
        ShowHover(true);
    }

    void OnMouseExit()
    {
        if (!selected) ShowHover(false);
    }

    void OnMouseUp()
    {
        WorldGen.SelectTile(this);
        selected = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool selected = false;
    private Collider col;
    private Color highlightColor;
    private Color stdColor;
    private MeshRenderer mesh;
    private float fadeSpeed = 2.5f;
    private float fadeDeadZone = .25f;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshRenderer>();

        stdColor = mesh.material.color;
        highlightColor = Color.red; //mesh.material.color*new Vector4(1.1f,1.1f,1.1f,1);
    }

    // Update is called once per frame
    void Update()
    {
        var currentColor = mesh.material.color;
        var targetColor = selected ? highlightColor : stdColor;

        if(currentColor != targetColor)
        {
            var snap = Vector4.Distance(currentColor,targetColor) < fadeDeadZone;
            mesh.material.color = !snap ? Color.Lerp(currentColor,targetColor,Time.deltaTime*fadeSpeed) : targetColor;
        }
    }

    public void ToggleSelect()
    {
        selected = !selected;

        
    }
}

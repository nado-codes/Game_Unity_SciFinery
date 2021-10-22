using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panelCreateParticle : MonoBehaviour
{
    public enum Particle {Proton, Neutron, Electron}

    private bool isHover = false;
    private bool isEdit = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        isHover = true;
    }

    void OnMouseExit()
    {
        if(!isEdit) isHover = false;
    }

    public void Create(Particle particle)
    {
        if(!isEdit)
        {
            Debug.Log("Creating a "+particle);
            isEdit = true;
        }
    }

    public void FinishCreate()
    {
        isEdit = false;
    }
}   

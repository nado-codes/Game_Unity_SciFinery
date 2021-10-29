using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Particle {Proton, Neutron, Electron}
    public enum CreationState{None, Start, Spawn, End}

public class panelCreateParticle : MonoBehaviour
{
    

    private bool isHover = false;
    public CreationState creationState = CreationState.None;
    public Particle particleToCreate = Particle.Proton;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButtonUp(0))
          //  handleSpawnParticle();
    }

    public void startDragParticle ()
    {
        if(creationState == CreationState.Start)
        {

        }
    }

    void OnMouseEnter()
    {
        isHover = true;
    }

    // TODO: spawn particle when mouse exits panel
    // TODO: drop particle when mouse is up and set state to NONE

    void OnMouseExit()
    {
        // if(!isEdit) isHover = false;
    }

    
    public void Create(Particle particle)
    {
        /* if(!isEdit)
        {
            Debug.Log("Creating a "+particle);
            isEdit = true;
        } */
    }


}   

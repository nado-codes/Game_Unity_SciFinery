using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ParticleType {Proton, Neutron, Electron}
    public enum CreationState{None, Start, Drag}

public class panelCreateParticle : MonoBehaviour, IPointerExitHandler
{
    

    private bool isHover = false;
    public CreationState creationState = CreationState.None;
    public ParticleType particleToCreate = ParticleType.Proton;
    private GameObject currentParticle;

    public GameObject proton;
    public GameObject neutron;
    public GameObject electron;

    public float particleDefaultDistance = 5;
    private float particleDistance = 5;
    private float zoomSensitivity = 200;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(creationState == CreationState.Drag)
        {
            // zoom
            float scroll = Input.mouseScrollDelta.y;

            bool isZoomIn = scroll < 0;
            bool isZoomOut = scroll > 0;

            if(isZoomIn)
                Debug.Log("zoom in");

                if(isZoomOut)
                Debug.Log("zoom out");

            if(isZoomOut && particleDistance < 50)
                particleDistance += zoomSensitivity*Time.deltaTime;

            if(isZoomIn && particleDistance > 5)
                particleDistance -= zoomSensitivity*Time.deltaTime;

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            currentParticle.transform.position = cameraRay.origin + (cameraRay.direction * particleDistance);

            if(Input.GetMouseButtonUp(0))
            {
                particleDistance = particleDefaultDistance;
                currentParticle = null;
                creationState = CreationState.None;
                Editor.SetDragSelectEnabled(true);
            }
        }
    }

    void OnMouseEnter()
    {
        isHover = true;
    }

    // TODO: spawn particle when mouse exits panel
    // TODO: drop particle when mouse is up and set state to NONE

    public void OnPointerExit(PointerEventData ev)
    {
        if(creationState == CreationState.Start)
        {
            Debug.Log("start drag");
            
            if(particleToCreate == ParticleType.Proton)
                currentParticle = Instantiate(proton);
            else if(particleToCreate == ParticleType.Neutron)
                currentParticle = Instantiate(neutron);
            else if(particleToCreate == ParticleType.Electron)
                currentParticle = Instantiate(electron);

            creationState = CreationState.Drag;
            Editor.SetDragSelectEnabled(false);
        }
        // if(!isEdit) isHover = false;
    }


}   

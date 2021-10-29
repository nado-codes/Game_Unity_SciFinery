using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class panelButton : MonoBehaviour, IPointerDownHandler
{
    private panelCreateParticle panel;
    

    public ParticleType particleType;

    void Start()
    {
        panel = GetComponentInParent<panelCreateParticle>();
    }

    public void OnPointerDown(PointerEventData ev)
    {
        if(panel.creationState == CreationState.None)
        {
            panel.particleToCreate = particleType;
            panel.creationState = CreationState.Start;
            Debug.Log("start create");
        }
    }

    void OnPointerExit()
    {
        Debug.Log("pointer has exited");
    }

    void OnPointerUp()
    {
        // panel.FinishCreate();
    }
}

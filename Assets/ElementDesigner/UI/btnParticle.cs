using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class btnParticle : MonoBehaviour, IPointerDownHandler
{
    private PanelCreate panel;


    public ParticleType particleType;

    void Start()
    {
        panel = GetComponentInParent<PanelCreate>();
    }

    public void OnPointerDown(PointerEventData ev)
    {
        if (panel.creationState == CreationState.None)
        {
            // panel.elementToCreate = particleType;
            panel.creationState = CreationState.Start;
            Debug.Log("start create");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class CoilAnim : MonoBehaviour
{
    private IEnumerable<MeshRenderer> coils;
    private int coilIndex = 0;
    private MeshRenderer currentCoilRenderer;
    private Color coilBaseColor;
    
    private DateTime referenceTime = DateTime.Now;
    private int animIntervalMS = 250;

    void Start()
    {
        coils = GetComponentsInChildren<MeshRenderer>().OrderBy(x => x.transform.position.y).ToList();
        
        foreach(MeshRenderer coil in coils)
            coil.material.SetColor("_EMISSION",coil.material.color);

        currentCoilRenderer = coils.ElementAt(coilIndex);
        coilBaseColor = currentCoilRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        var currentTime = DateTime.Now;

        if(currentTime.Subtract(referenceTime).Milliseconds > animIntervalMS)
        {
            currentCoilRenderer.material.DisableKeyword("_EMISSION");
            
            coilIndex = (coilIndex < coils.Count()-1) ? coilIndex + 1 : 0;

            currentCoilRenderer = coils.ElementAt(coilIndex);
            currentCoilRenderer.material.EnableKeyword("_EMISSION");

            referenceTime = currentTime;
        }
    }
}

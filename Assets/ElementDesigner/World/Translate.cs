using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : MonoBehaviour
{
    public static bool IsActive {get; private set;}
    TranslateHandle handle;
    Transform centerTransform;
    Ray cameraStartRay;
    Vector3 cameraStartPosition => cameraStartRay.origin;
    public int zoomAmount = 25;
    Vector3 zoomOffsetTarget;
    Vector3 zoomOffset;
    Vector3 handleOffsetPosition;
    Vector3 handleOffsetTarget;
    Vector3 referencePosition;

    // Start is called before the first frame update
    void Start()
    {
        handle = GetComponentInChildren<TranslateHandle>();
        centerTransform = transform.Find("Brake");
    }

    void Update()
    {
        if(handle.IsActive)
        {
            

            Ray cameraCurrentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 cameraCurrentPosition = cameraCurrentRay.origin;
            Vector3 translatePosition = cameraCurrentPosition-cameraStartPosition;

            Vector3 translateDirection = cameraCurrentRay.direction-cameraStartRay.direction;
            Vector3 cameraDirection = cameraCurrentRay.origin-cameraStartRay.origin;

            // zoom
            float scroll = Input.mouseScrollDelta.y;

            bool isZoomIn = scroll > 0;
            bool isZoomOut = scroll < 0;

            Vector3 zoomDirection = cameraCurrentRay.direction+Camera.main.transform.forward;
            // Vector3 translateDirection
            //handleOffsetPosition = Vector3.Lerp(handleOffsetPosition,handleOffsetTarget,Time.deltaTime);

            if(isZoomIn)
                zoomOffsetTarget += (zoomDirection*zoomAmount*4*Time.deltaTime);
        
            if(isZoomOut)
                zoomOffsetTarget += (-zoomDirection*zoomAmount*4*Time.deltaTime);
            
            zoomOffset = Vector3.Lerp(zoomOffset,zoomOffsetTarget,Time.deltaTime);

            if(Vector3.Distance(centerTransform.position,handle.transform.position) < 3)
            {   
                referencePosition = handle.startPosition + (translateDirection * 1) + (cameraDirection* .8f);
            }

            handle.transform.position = referencePosition + zoomOffset;
        }
    }

    public void SetTranslateIsActive(bool active)
    {
        if(active)
        {
            cameraStartRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            zoomOffsetTarget = Vector3.zero;
            zoomOffset = Vector3.zero;
            handleOffsetTarget = Vector3.zero;
            handleOffsetPosition = Vector3.zero;
        }

        IsActive = active;
    }
}

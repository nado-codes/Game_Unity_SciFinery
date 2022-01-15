using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Translate : MonoBehaviour
{
    public static bool IsActive {get; private set;}
    TranslateHandle handle;
    TranslateBrake brake;
    Transform centerTransform;
    Ray cameraStartRay;
    Vector3 cameraStartPosition => cameraStartRay.origin;
    public int zoomAmount = 25;
    Vector3 zoomOffsetTarget;
    Vector3 zoomOffset;
    Vector3 handleOffsetPosition;
    Vector3 handleOffsetTarget;
    Vector3 referencePosition;

    public static Translate Instance;

    // Start is called before the first frame update
    void Start()
    {
        handle = GetComponentInChildren<TranslateHandle>();
        brake = GetComponentInChildren<TranslateBrake>();
        centerTransform = transform.Find("Brake");
        SetActive(false);
    }

    void Awake()
    {
        Instance = this;
    }

    public static void SetActive(bool active) => Instance.gameObject.SetActive(active);

    ///<Summary>  
    /// All selected objects in the Editor will have their velocity increased, where applicable, in the direction specified by [dir]
    /// </Summary>
    public void Accelerate(Vector3 dir)
    {
        Debug.Log("Accelerating "+Editor.SelectedObjects.Count()+" particles...");

        // Editor.SelectedObjects.ToList().ForEach(s => s.GetComponent<Particle>().AddVelocity(dir,3));
    }

    ///<Summary>  
    /// All selected objects in the Editor will have their velocity decreased, where applicable, in the direction specified by [dir]
    /// </Summary>
    public void Brake()
    {
        Debug.Log("Slowing down "+Editor.SelectedObjects.Count()+" particles...");
        // Editor.SelectedObjects.ToList().ForEach(s => s.GetComponent<Particle>().Brake(.1f));
    }

    ///<Summary>  
    /// All selected objects in the Editor will be decelerated to a complete stop, until another acceleration is performed. Deceleration may be interupted.
    /// </Summary>
    public void AllStop()
    {
        Debug.Log("ALL STOP");
        // Editor.SelectedObjects.ToList().ForEach(s => s.GetComponent<Particle>().AllStop());
    }

    void Update()
    {
        /* if(brake.IsActive)
        {
            Brake();
        }
        if(handle.IsActive)
        {
            Ray cameraCurrentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 cameraCurrentPosition = cameraCurrentRay.origin;
            Vector3 translatePosition = cameraCurrentPosition-cameraStartPosition;

            Vector3 translateDirection = cameraCurrentRay.direction-cameraStartRay.direction;
            Vector3 cameraDirection = cameraCurrentRay.origin-cameraStartRay.origin;

            Accelerate(translateDirection+(cameraDirection*.8f));

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
                referencePosition = handle.startLocalPosition + (translateDirection * 1) + (cameraDirection* .8f);
            }

            handle.transform.position = referencePosition + zoomOffset;
        } */
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

    void OnMouseDown()
    {
        SetTranslateIsActive(true);
    }

    void OnMouseUp()
    {
        SetTranslateIsActive(false);
    }
}

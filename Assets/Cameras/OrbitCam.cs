using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    private Transform trackedObject;
    public Transform TrackedObject
    {
        get => trackedObject;
        set
        {
            transform.parent = value;
            trackedObject = value;
        }
    }

    private Vector3 _trackedPosition;
    private Vector3 trackedPosition => TrackedObject?.position ?? Vector3.zero;

    private float xAngle = 0;
    private float yAngle = 0;
    private float zoomAmount = 10;
    public float lookSensitivity = 10;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            xAngle += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            yAngle -= Input.GetAxis("Mouse Y") * lookSensitivity * .5f * Time.deltaTime;
            yAngle = Mathf.Clamp(yAngle, -1, 1);
        }

        if (TrackedObject != null)
        {
            UpdateOrbit();
            HUD.LockFocus();
        }

        UpdateZoom();
    }

    void UpdateOrbit()
    {
        var xPosH = Mathf.Sin(xAngle) * zoomAmount;
        var zPosH = Mathf.Cos(xAngle) * zoomAmount;
        var yPosV = Mathf.Sin(yAngle) * zoomAmount;
        var posH = new Vector3(xPosH, yPosV, zPosH);
        transform.localPosition = posH;
        transform.LookAt(trackedPosition);
    }

    void UpdateZoom()
    {
        var scroll = Input.mouseScrollDelta.y;
        var isZoomIn = scroll < 0;
        var isZoomOut = scroll > 0;
        var zoomFactor = 100 + (zoomAmount * .003f);

        if (isZoomIn)
            zoomAmount += zoomFactor * Time.deltaTime;
        if (isZoomOut)
            zoomAmount -= zoomFactor * Time.deltaTime;

        zoomAmount = Mathf.Clamp(zoomAmount, 5, 50);
    }
}
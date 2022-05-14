using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    public Transform TrackedObject { get; set; }
    private Vector3 trackedObjectPosition => TrackedObject?.position ?? Vector3.zero;

    private float xAngle = 0;
    private float yAngle = 0;
    private Vector3 localRotation;

    void Start()
    {

    }

    void Update()
    {
        //if (Input.GetMouseButton(1))
        UpdateAim();

        yAngle = Mathf.Clamp(yAngle, 0, 90);

        var xPosH = Mathf.Sin(xAngle) * 10;
        var zPosH = Mathf.Cos(xAngle) * 10;
        var posH = new Vector3(xPosH, 0, zPosH);

        var yPosV = Mathf.Sin(yAngle) * 10;
        var zPosV = Mathf.Cos(yAngle) * 10;
        var posV = new Vector3(0, yPosV, 0) + (transform.forward * zPosV);

        transform.position = trackedObjectPosition + posH + posV;
        transform.LookAt(trackedObjectPosition);

        /* if (mouseX != 0 || mouseY != 0)
        {
            localRotation.x += mouseX * 10;
            localRotation.y -= mouseY * 10;

            localRotation.y = Mathf.Clamp(localRotation.y, 0, 90f);
        }

        float cameraDistance = 0;

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * 10;
            scrollAmount *= (cameraDistance * .3f);

            cameraDistance += scrollAmount * -1f;
            // .. no closer than 1.5m, no further than 100m
            cameraDistance = Mathf.Clamp(cameraDistance, 1.5f, 100f);
        }

        Quaternion QT = Quaternion.Euler(localRotation.y,localRotation.x,0);
        transform */
    }

    void UpdateAim()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        xAngle += mouseX * Time.deltaTime;
        yAngle -= mouseY * Time.deltaTime;

    }
}
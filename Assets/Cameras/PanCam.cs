using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCam : MonoBehaviour
{
    private Vector3 speed = Vector3.zero;
    public Vector3 speedDeadZone = new Vector3(.1f,.1f,0);
    public float acceleration = 0.1f;
    public float edgeSize = 25;
    public float maxSpeed = 5;
    public float braking = 1;
    public int zoomAmount = 25;
    private bool isIdleX = false;
    private bool isIdleY = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float GetRealSpeed(float speed)
        => speed < 0 ? -speed : speed;

    // Update is called once per frame
    void Update()
    {
        // movement
        var myForward = new Vector3(-transform.forward.z,0,-transform.forward.x);
        var myRight = new Vector3(-transform.right.x,0,-transform.right.z);

        bool panForward = Input.GetKey(KeyCode.W) || Input.mousePosition.y > (Screen.height-edgeSize);
        bool panBackward = Input.GetKey(KeyCode.S) || Input.mousePosition.y < edgeSize;
        bool panLeft = Input.GetKey(KeyCode.A) || Input.mousePosition.x < edgeSize;
        bool panRight = Input.GetKey(KeyCode.D) || Input.mousePosition.x > (Screen.width-edgeSize);
        bool isSprint = Input.GetKey(KeyCode.LeftShift);

        // zoom
        float scroll = Input.mouseScrollDelta.y;

        bool isZoomIn = scroll > 0;
        bool isZoomOut = scroll < 0;

        if(isZoomIn)
            transform.Translate(Vector3.forward*zoomAmount*4*Time.deltaTime);
        
        if(isZoomOut)
            transform.Translate(Vector3.back*zoomAmount*4*Time.deltaTime);

        // movement
        if(panForward && speed.y < maxSpeed) speed.y += acceleration * (isSprint ? 2 : 1);
        if(panBackward && speed.y > -maxSpeed) speed.y -= acceleration * (isSprint ? 2 : 1);
        if(panLeft && speed.x < maxSpeed) speed.x += acceleration * (isSprint ? 2 : 1);
        if(panRight && speed.x > -maxSpeed) speed.x -= acceleration * (isSprint ? 2 : 1);

        // braking & cursor
        isIdleX = !panLeft && !panRight;
        isIdleY = !panForward && !panBackward;

        if(isIdleX) speed.x = GetRealSpeed(speed.x) > speedDeadZone.x ? Mathf.Lerp(speed.x,speed.x*.9f,braking*.1f) : 0;
        if(isIdleY) speed.y = GetRealSpeed(speed.y) > speedDeadZone.y ? Mathf.Lerp(speed.y,speed.y*.9f,braking*.1f) : 0;

        // translation
        transform.Translate(myForward*speed.y*Time.deltaTime,Space.World);
        transform.Translate(myRight*speed.x*Time.deltaTime,Space.World);
    }
}

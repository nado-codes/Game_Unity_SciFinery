using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 speed = Vector3.zero;
    public Vector3 speedDeadZone = new Vector3(.1f,.1f,0);
    public float acceleration = 0.1f;
    public float braking = 1;
    public int zoomAmount = 25;
    private float lastZoom = 0;
    private bool shouldBrakeX = false;
    private bool shouldBrakeY = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float GetRealSpeed(float speed)
        => speed < 0 ? -speed : speed;

    // Update is called once per frame
    void Update()
    {
        var panForward = new Vector3(-transform.forward.z,0,-transform.forward.x);
        var panRight = new Vector3(-transform.right.x,0,-transform.right.z);

        bool isW = Input.GetKey(KeyCode.W);
        bool isS = Input.GetKey(KeyCode.S);
        bool isA = Input.GetKey(KeyCode.A);
        bool isD = Input.GetKey(KeyCode.D);
        bool isSprint = Input.GetKey(KeyCode.LeftShift);

        
        float scroll = Input.mouseScrollDelta.y;

        bool isZoomIn = scroll > lastZoom;
        bool isZoomOut = scroll < lastZoom;

        lastZoom = scroll;

        if(isZoomIn)
            transform.Translate(Vector3.forward*zoomAmount*4*Time.deltaTime);
        
        if(isZoomOut)
            transform.Translate(Vector3.back*zoomAmount*4*Time.deltaTime);

        if(isW) speed.y += acceleration * (isSprint ? 2 : 1);
        if(isS) speed.y -= acceleration * (isSprint ? 2 : 1);
        if(isA) speed.x += acceleration * (isSprint ? 2 : 1);
        if(isD) speed.x -= acceleration * (isSprint ? 2 : 1);

        shouldBrakeX = !isA && !isD;
        shouldBrakeY = !isW && !isS;

        if(shouldBrakeX) speed.x = GetRealSpeed(speed.x) > speedDeadZone.x ? Mathf.Lerp(speed.x,speed.x*.9f,braking*.1f) : 0;
        if(shouldBrakeY) speed.y = GetRealSpeed(speed.y) > speedDeadZone.y ? Mathf.Lerp(speed.y,speed.y*.9f,braking*.1f) : 0;

        transform.Translate(panForward*speed.y*Time.deltaTime,Space.World);
        transform.Translate(panRight*speed.x*Time.deltaTime,Space.World);
    }
}

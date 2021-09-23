using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 speed = Vector3.zero;
    private float acceleration = 0.1f;
    private bool isAccelerating = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var panForward = new Vector3(-transform.forward.z,0,-transform.forward.x);
        var panRight = new Vector3(-transform.right.x,0,-transform.right.z);

        bool isW = Input.GetKey(KeyCode.W);
        bool isS = Input.GetKey(KeyCode.S);
        bool isA = Input.GetKey(KeyCode.A);
        bool isD = Input.GetKey(KeyCode.D);

        if(isW) speed.y += acceleration * Time.deltaTime;
        if(isS) speed.y -= acceleration * Time.deltaTime;
        if(isA) speed.x += acceleration * Time.deltaTime;
        if(isD) speed.x -= acceleration * Time.deltaTime;

        isAccelerating = isW || isS  || isA || isD;

        //TODO: Implement deceleration when releasing the movement keys for each axis (if no left/right, decelerate)
        //(if no forward back, decelerate)

        //if(!isAccelerating) speed = Vector3.Lerp(speed,speed*(float)(1-acceleration*0.1),Time.deltaTime);
        Debug.Log(speed);

        transform.Translate(panForward*speed.y,Space.World);
        transform.Translate(panRight*speed.x,Space.World);
    }
}

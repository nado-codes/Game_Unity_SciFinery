using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour {

    float xSense = 300, ySense = 300;
    float speed = 30, speedActual = 0, speedStrafe = 0, speedLev = 0;
    int dir = 1, strafeDir = 1, levDir = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        UpdateAim();
        UpdateMovement();

    }

    void UpdateAim()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * xSense, 0) * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -ySense, 0, 0) * Time.deltaTime, Space.Self);
        }
    }

    void UpdateMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
                dir = 1;
            else if (Input.GetKey(KeyCode.S))
                dir = -1;

            speedActual = speed;
        }
        else
            speedActual = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
                strafeDir = -1;
            else if (Input.GetKey(KeyCode.D))
                strafeDir = 1;

            speedStrafe = speed;
        }
        else
            speedStrafe = 0;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            if (Input.GetKey(KeyCode.Q))
                levDir = -1;
            else if (Input.GetKey(KeyCode.E))
                levDir = 1;

            speedLev = speed;
        }
        else
            speedLev = 0;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = Mathf.Lerp(speed,speed*1.8f,Time.deltaTime*1.1f);
        else
            speed = 10;

        transform.Translate(Vector3.forward * speedActual * dir * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.right * speedStrafe * strafeDir * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.up * speedLev * levDir * Time.deltaTime, Space.Self);
    }
}

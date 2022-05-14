using UnityEngine;
using System;
using System.Collections;

public class FlyCam : MonoBehaviour
{

    private float xSensitivity = 300, ySensitivity = 300;
    public float baseSpeed = 10, accelerationMultiplier = 1f;
    private float speedSprint = 0;
    int forwardDir = 1, strafeDir = 1, levDir = 1;
    public Vector3 Velocity { get; private set; }

    private DateTime lastMovementTime;

    // Use this for initialization
    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (!HUD.LockedFocus)
            UpdateAim();

        UpdateMovement();
    }

    void UpdateAim()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * xSensitivity, 0) * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -ySensitivity, 0, 0) * Time.deltaTime, Space.Self);
        }
    }

    void UpdateMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
                forwardDir = 1;
            else if (Input.GetKey(KeyCode.S))
                forwardDir = -1;

            lastMovementTime = DateTime.Now;
        }
        else
            forwardDir = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
                strafeDir = -1;
            else if (Input.GetKey(KeyCode.D))
                strafeDir = 1;

            lastMovementTime = DateTime.Now;
        }
        else
            strafeDir = 0;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            if (Input.GetKey(KeyCode.Q))
                levDir = 1;
            else if (Input.GetKey(KeyCode.E))
                levDir = -1;

            lastMovementTime = DateTime.Now;
        }
        else
            levDir = 0;

        if (Input.GetKey(KeyCode.LeftShift))
            speedSprint = Mathf.Lerp(speedSprint, speedSprint * accelerationMultiplier, Time.deltaTime);

        if (!Input.GetKey(KeyCode.LeftShift) || (DateTime.Now - lastMovementTime).TotalMilliseconds > 100)
            speedSprint = baseSpeed;

        var forwardVelocity = Vector3.forward * baseSpeed * speedSprint * forwardDir;
        var strafeVelocity = Vector3.right * baseSpeed * speedSprint * strafeDir;
        var verticalVelocity = Vector3.up * baseSpeed * speedSprint * levDir;
        Velocity = forwardVelocity + strafeVelocity + verticalVelocity;

        transform.Translate(forwardVelocity * Time.deltaTime);
        transform.Translate(strafeVelocity * Time.deltaTime);
        transform.Translate(verticalVelocity * Time.deltaTime);
    }
}

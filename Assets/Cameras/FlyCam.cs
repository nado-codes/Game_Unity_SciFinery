using UnityEngine;
using System;
using System.Collections.Generic;

public enum MouseButton { Left = 0, Right = 1 }
public enum Direction { Forward = 0, Back = 1, Left = 2, Right = 3, Up = 4, Down = 5 }
public class FlyCam : MonoBehaviour
{

    private float xSensitivity = 300, ySensitivity = 300;
    public float baseSpeed = 10, accelerationMultiplier = 1f;
    private float speedSprint = 0;
    int forwardDir = 1, strafeDir = 1, levDir = 1;
    public Vector3 Velocity { get; private set; }
    public MouseButton cameraMouseButton = MouseButton.Right;
    public bool LockAim = false;
    private Dictionary<Direction, bool> manualMove = new Dictionary<Direction, bool>() {
        {Direction.Forward, false},
        {Direction.Back, false},
        {Direction.Left, false},
        {Direction.Right,false}
    };

    private DateTime lastMovementTime;

    // Use this for initialization
    protected void Start()
    {
        // TODO: "Multi-input for touch devices"
        /* var movementButtonsTransform = GameObject.Find("movementButtons").transform;
        var btnFwd = movementButtonsTransform?.Find("btnForward");
        Assertions.AssertNotNull(btnFwd, "btnFwd");
        var btnBack = movementButtonsTransform?.Find("btnBackward");
        Assertions.AssertNotNull(btnBack, "btnBack");
        var btnLeft = movementButtonsTransform?.Find("btnLeft");
        Assertions.AssertNotNull(btnLeft, "btnLeft");
        var btnRight = movementButtonsTransform?.Find("btnRight");
        Assertions.AssertNotNull(btnRight, "btnRight"); */
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!LockAim)
            UpdateAim();

        UpdateMovement();
    }

    void UpdateAim()
    {
        if (Input.GetMouseButton((int)cameraMouseButton))
        {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * xSensitivity, 0) * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -ySensitivity, 0, 0) * Time.deltaTime, Space.Self);
        }
    }

    void UpdateMovement()
    {
        forwardDir = 0;
        strafeDir = 0;
        levDir = 0;

        // var isForward = Input.GetKey(KeyCode.W) || manualMove[Direction.Forward];

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
                forwardDir = 1;
            else if (Input.GetKey(KeyCode.S))
                forwardDir = -1;

            lastMovementTime = DateTime.Now;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
                strafeDir = -1;
            else if (Input.GetKey(KeyCode.D))
                strafeDir = 1;

            lastMovementTime = DateTime.Now;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            if (Input.GetKey(KeyCode.Q))
                levDir = 1;
            else if (Input.GetKey(KeyCode.E))
                levDir = -1;

            lastMovementTime = DateTime.Now;
        }

        if (forwardDir != 0 || strafeDir != 0 || levDir != 0)
        {
            EditorSelect.SetDragSelectEnabled(true);

            var orbitCam = Camera.main.GetComponent<OrbitCam>();

            if (orbitCam != null)
            {
                var cameraTransform = Camera.main.transform;
                cameraTransform.parent = cameraTransform.parent == orbitCam.TrackedObject ? null : cameraTransform.parent;
                orbitCam.TrackedObject = null;
                LockAim = false;
            }
        }

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

    public void Move(Direction dir, bool active)
    {
        manualMove[dir] = active;
    }
}

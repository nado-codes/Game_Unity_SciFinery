using UnityEngine;
using System.Collections;

public class CameraFacer : MonoBehaviour
{

    Camera mainCam;

    // Use this for initialization
    void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 v = Camera.main.transform.position - transform.position;

        // v.z = 0.0f;

        v.x = v.z = 0.0f;


        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }
}

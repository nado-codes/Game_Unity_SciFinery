using UnityEngine;
using System.Collections;
using UnityCamera = UnityEngine.Camera;

public class CameraFacer : MonoBehaviour
{

    UnityCamera mainCam;

    // Use this for initialization
    void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<UnityCamera>();
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

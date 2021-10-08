using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleus : MonoBehaviour
{
    private Transform _cube;
    // Start is called before the first frame update
    void Start()
    {
        _cube = transform.Find("Cube");
    }

    float t = 0;

    // Update is called once per frame
    void Update()
    {
        var speed = 500;
        t += 1 * Time.deltaTime;
        
        var rot = new Vector3(Mathf.Sin(t),1,Mathf.Cos(t));

        //transform.RotateAround(_cube.position,rot,speed * Time.deltaTime);
        //transform.Rotate(rot*speed*Time.deltaTime,Space.World);
    }
}

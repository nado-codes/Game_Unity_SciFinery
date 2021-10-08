using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proton : Particle
{
    private Nucleus _nucleus;
    private Light _light;
    private Vector3 _velocity;
    private Vector3 _orbitOrigin;
    private bool frozen = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _nucleus = transform.parent.GetComponentInChildren<Nucleus>();
        _light = GetComponentInChildren<Light>();

        var v = (_nucleus.transform.position- transform.position);
        _orbitOrigin = new Vector3(v.x,-v.y,v.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_nucleus.transform.position,Vector3.up,200*Time.deltaTime);
        /* if(!frozen)
        {
            // ApplyGravity();

            

            transform.Translate(_velocity * Time.deltaTime);

            if(Vector3.Distance(transform.position,_nucleus.transform.position) < 1)
            {
                frozen = true;

                _orbitOrigin = transform.position - _nucleus.transform.position;
            }
        }
        else
        {
            PulseLight();
        } */
    }

    private float t = 0;
    void PulseLight()
    {
        var speed = 4;
        t += Time.deltaTime*speed;
        var intensity = (Mathf.Sin(t)+1)/2;

        _light.intensity = intensity;
    }

    void ApplyGravity()
    {
        var v = _nucleus.transform.position-transform.position;


        _velocity += v.normalized * 10 * Time.deltaTime;
    }


}

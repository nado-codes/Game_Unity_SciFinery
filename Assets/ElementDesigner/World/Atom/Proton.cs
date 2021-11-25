using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proton : Particle
{
    private Nucleus _nucleus;
    private Light _light;
    private Vector3 _velocity;
    private Vector3 _orbitOrigin;
    public Vector3 Axis = new Vector3(0,1,0);
    private bool frozen = false;
    

    // Start is called before the first frame update
    void Start()
    {
        // _nucleus = transform.parent.GetComponentInChildren<Nucleus>();
        _light = GetComponentInChildren<Light>();

        // var v = (_nucleus.transform.position- transform.position).normalized;
        // _orbitOrigin = new Vector3(v.x,-v.y,v.z);
    }

    // Update is called once per frame
    new void Update()
    {
        // transform.RotateAround(_nucleus.transform.position,Axis,200*Time.deltaTime);
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

        base.Update();
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

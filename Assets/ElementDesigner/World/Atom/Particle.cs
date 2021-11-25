using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private bool allStopIsActive = false;

    void Start()
    {
        
    }

    protected void Update()
    {
        transform.Translate(velocity*Time.deltaTime);

        Debug.Log("vel of "+gameObject.name+"="+velocity);

        if(allStopIsActive)
            velocity = Vector3.Lerp(velocity,velocity*.9f,Time.deltaTime);
    }

    public Vector3 AddVelocity(Vector3 dir, float amount)
    {
        Debug.Log("adding "+amount+" points of velocity in direction "+dir);
        velocity += dir * amount * Time.deltaTime;
        allStopIsActive = false;

        return velocity;
    }

    public Vector3 Brake(float amount)
    {
        if(!allStopIsActive)
            velocity *= (1-amount);

        return velocity;
    }

    public bool AllStop()
    {
        allStopIsActive = true;

        return allStopIsActive;
    }
}
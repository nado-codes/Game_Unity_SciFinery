using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public enum Charge{Positive = 1, None = 0, Negative = -1}
    private Vector3 velocity = Vector3.zero;
    private bool allStopIsActive = false;

    public Charge charge = Charge.None;
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    // priv

    protected void Update()
    {
        transform.Translate(velocity*Time.deltaTime);

        // Debug.Log("vel of "+gameObject.name+"="+velocity);
        
        // Apply charges
        var worldParticles = World.Particles.Where(x => x != this);
        // Debug.Log(gameObject.name+" checking "+worldParticles.Count()+" other particles");

        var velocityDirection = Vector3.zero;
        worldParticles.ToList().ForEach(x => {
            var effectiveCharge = (int)x.charge*(int)charge;
            var xBody = x.transform.Find("Body");
            var body = transform.Find("Body");
            var sizeOffset = xBody.lossyScale.magnitude / body.lossyScale.magnitude;

            // .. remove this to re-enable repulsive forces
            effectiveCharge = effectiveCharge == 1 ? -1 : effectiveCharge;

            var dirTo = transform.position-x.transform.position;
            velocityDirection += dirTo * effectiveCharge * sizeOffset;
        });

        velocity += velocityDirection * Time.deltaTime * .5f;

        // .. this keeps the particles within a certain range of the center
         if(Vector3.Distance(transform.position,Vector3.zero) > 40)
        {
            // transform.position = transform.position - (transform.position-startPosition).normalized*.1f;
            // velocity = -velocity;
        }

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
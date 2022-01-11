using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;

public enum ParticleType {Proton, Neutron, Electron}

public class Particle : MonoBehaviour
{
    public enum Charge{Positive = 1, None = 0, Negative = -1}
    private Vector3 velocity = Vector3.zero;
    private bool allStopIsActive = false;

    public Charge charge = Charge.None;
    public ParticleType type = ParticleType.Proton;

    public float massMultiplier = 1;
    Vector3 apoapsis;
    Vector3 periapsis;
    Vector3 startPosition;

    protected void Start()
    {
        apoapsis = transform.position;
        periapsis = transform.position;
        startPosition = transform.position;

        // .. Work out which particle is closest to orbit
        // .. Is this obsolete? Probably needs a refactor at least
        var closest = Editor.OtherParticles(this).Aggregate((closest,next) => 
            Vector3.Distance(next.transform.position, transform.position) <
            Vector3.Distance(closest.transform.position, transform.position) ?
            next : closest
        );

        if(closest != null)
        {
            var xBody = closest.transform.Find("Body");
            var body = transform.Find("Body");
            var massOffset =  1/ (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * massMultiplier;
            var distanceOffset = 10* (1 / Vector3.Distance(xBody.transform.position,transform.position));

            apoapsis = (transform.position-closest.transform.position) * massOffset * distanceOffset;
            periapsis = apoapsis*2;
        }
    }

    // priv

    protected void Update()
    {
        transform.Translate(velocity*Time.deltaTime);

        // Debug.Log("vel of "+gameObject.name+"="+velocity);
        
        // Apply charges
        var worldParticles = Editor.Particles.Where(x => x != this);
        // Debug.Log(gameObject.name+" checking "+worldParticles.Count()+" other particles");

        var effectiveForce = Vector3.zero;
        worldParticles.ToList().ForEach(x => {
            var effectiveCharge = (int)x.charge*(int)charge;
            var xBody = x.transform.Find("Body");
            var body = transform.Find("Body");
            var massOffset =  1/ (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * massMultiplier;
            var distanceOffset = 10* (1 / Vector3.Distance(xBody.transform.position,transform.position));

            // .. comment this out to enable repulsive forces
            //effectiveCharge = effectiveCharge == 1 ? -1 : effectiveCharge;

            var dirTo = transform.position-x.transform.position;
            effectiveForce += dirTo * effectiveCharge * massOffset * distanceOffset;

            // Debug.DrawRay(transform.position,-dirTo,Color.yellow,.01f);
            
        });

        Debug.DrawRay(startPosition,apoapsis,Color.red,.01f);
        // Debug.DrawRay(startPosition,-periapsis,Color.green,.01f);
        

        velocity += effectiveForce * Time.deltaTime * .5f;

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public enum ParticleType {Proton, Neutron, Electron}

public class Particle : MonoBehaviour
{
    public enum Charge{Positive = 1, None = 0, Negative = -1}
    private Vector3 velocity = Vector3.zero;

    public Charge charge = Charge.None;
    public ParticleType type = ParticleType.Proton;

    private Canvas signCanvas;
    private Text textSign;

    public float massMultiplier = 1;

    protected void Start() {
        textSign = GetComponentInChildren<Text>();
        signCanvas = GetComponentInChildren<Canvas>();
    }

    protected void Update()
    {
        transform.Translate(velocity*Time.deltaTime);

        if(textSign != null && signCanvas != null)
        {
            var signCanvasRect = signCanvas.GetComponent<RectTransform>();
            var body = transform.Find("Body");
            
            var dist = Vector3.Distance(transform.position,Camera.main.transform.position)*.075f;
            signCanvasRect.localScale = new Vector3(1+dist,1+dist,1+dist)*(1/body.localScale.magnitude);
        }
        
        // Apply charges
        var worldParticles = Editor.Particles.Where(x => x != this);

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
        });
        
        velocity += effectiveForce * Time.deltaTime * .5f;
    }

    // .. a product of the charge of and distance between this particle and an atom
    /* public float EffectiveChargeTo(Atom atom)
    {
        var distanceOffset = (1 / Vector3.Distance(atom.transform.position,transform.position));

        var effCharge = ((float)charge * distanceOffset);
        Debug.Log($"Effective charge for {gameObject.name} to {atom.Name} is {effCharge}");

        return (float)charge * distanceOffset;
    } */
}
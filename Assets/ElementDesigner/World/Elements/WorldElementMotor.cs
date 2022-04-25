using UnityEngine;
using System.Linq;
using System;

public class WorldElementMotor : MonoBehaviour
{
    private WorldElement worldElement;
    private WorldElement WorldElement
    {
        get
        {
            if (worldElement == null)
                worldElement = GetComponent<WorldElement>();
            if (worldElement == null)
                throw new ApplicationException("WorldElementMotor requires a WorldElement in order to work correctly. Please add one first.");

            return worldElement;
        }
    }
    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => new Vector3(velocity.x, velocity.y, velocity.z);

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        // Apply charges
        var worldMotors = Editor.SubElements.Select(el => el.GetComponent<WorldElementMotor>());
        var otherWorldMotors = worldMotors.Where(x => x != this).ToList();

        var effectiveForce = Vector3.zero;
        otherWorldMotors.ForEach(x =>
        {
            var effectiveCharge = x.Charge * Charge;
            var xBody = x.transform.Find("Body");
            var body = transform.Find("Body");
            var massOffset = 1 / (body.lossyScale.magnitude / xBody.lossyScale.magnitude) * massMultiplier;

            var distanceToParticle = Vector3.Distance(xBody.transform.position, transform.position);
            var distanceOffset = 1 / (distanceToParticle > 0 ? distanceToParticle : 1);

            // .. comment this out to enable repulsive forces
            //effectiveCharge = effectiveCharge == 1 ? -1 : effectiveCharge;

            var dirTo = transform.position - x.transform.position;
            effectiveForce += dirTo * effectiveCharge * massOffset * distanceOffset;
        });

        velocity += effectiveForce * Time.deltaTime;

    }
}
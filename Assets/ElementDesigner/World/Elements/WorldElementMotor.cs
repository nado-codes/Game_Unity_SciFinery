using UnityEngine;
using System.Linq;
using System;

public class WorldElementMotor : MonoBehaviour
{
    private WorldElement worldElement;
    public WorldElement WorldElement
    {
        get
        {
            if (worldElement == null)
                worldElement = GetComponent<WorldElement>();
            if (worldElement == null)
                throw new ApplicationException("WorldElementMotor requires a WorldElement to work properly. Please add one first.");

            return worldElement;
        }
    }

    private WorldElementReactor reactor => GetComponent<WorldElementReactor>();

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => new Vector3(velocity.x, velocity.y, velocity.z);

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (reactor == null || !reactor.IsFused)
            UpdateVelocity();
    }

    void UpdateVelocity()
    {
        try
        {
            // .. TODO: this method will break if two elements merge together e.g. transform.parent = newparent
            // .. Q: does changing parent in unity delete the original object, or does it move it?
            var worldMotors = Editor.SubElements.Where(e => e.GetComponent<WorldElementMotor>() != null);
            var otherWorldMotors = worldMotors.Where(x => x.GetComponent<WorldElementMotor>() != this).ToList();

            var effectiveForce = Vector3.zero;
            otherWorldMotors.ForEach(otherElement =>
            {
                var forceBetween = WorldElement.ForceBetween(otherElement);
                effectiveForce += forceBetween;
            });

            velocity += effectiveForce * Time.deltaTime;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void AddVelocity(Vector3 force)
    {
        velocity += force;
    }

    public void Stop()
    {
        velocity = Vector3.zero;
    }
}
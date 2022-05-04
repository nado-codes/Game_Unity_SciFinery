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
    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => new Vector3(velocity.x, velocity.y, velocity.z);

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        // Apply charges
        var worldMotors = Editor.SubElements;
        var otherWorldMotors = worldMotors.Where(x => x != this).ToList();

        var effectiveForce = Vector3.zero;
        otherWorldMotors.ForEach(otherElement =>
        {
            var forceBetween = WorldElement.ForceBetween(otherElement);
            effectiveForce += forceBetween;
        });

        velocity += effectiveForce * Time.deltaTime;

    }

    public void AddVelocity(Vector3 force)
    {
        velocity += force;
    }
}
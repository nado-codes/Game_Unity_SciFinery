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

    private ArcLight _arcLight;
    private ArcLight arcLight
    {
        get
        {
            if (_arcLight == null)
            {
                var bodyTransform = transform.Find("Body");
                var arcLightTransform = bodyTransform?.Find("ArcLight");
                _arcLight = arcLightTransform?.GetComponent<ArcLight>();
            }
            if (_arcLight == null && WorldElement.Data.ElementType == ElementType.Particle)
                throw new ApplicationException("WorldElementMotor requires an ArcLight to work properly. Please add one first.");

            return _arcLight;
        }
    }

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => new Vector3(velocity.x, velocity.y, velocity.z);

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        // Apply charges
        var worldMotors = Editor.SubElements.Where(e => e.GetComponent<WorldElementMotor>() != null);
        var otherWorldMotors = worldMotors.Where(x => x.GetComponent<WorldElementMotor>() != this).ToList();

        var effectiveForce = Vector3.zero;
        otherWorldMotors.ForEach(otherElement =>
        {
            var forceBetween = WorldElement.ForceBetween(otherElement);

            // .. whether or not THIS and the other element should be attracted to each other
            var bothElementsNucleic = WorldElement.Data.Charge >= 0 && otherElement.Data.Charge >= 0;
            var withinNuclearRange = Vector3.Distance(WorldElement.transform.position, otherElement.transform.position) < 2;
            var useNuclear = withinNuclearRange && bothElementsNucleic;

            if (useNuclear && !arcLight.Active)
                arcLight.SetActive(true);
            if (!useNuclear && arcLight.Active)
                arcLight.SetActive(false);

            var dirTo = (WorldElement.transform.position - otherElement.transform.position).normalized;
            arcLight.transform.position = dirTo;

            if (!bothElementsNucleic)
                effectiveForce += forceBetween;
            else if (useNuclear)
                effectiveForce -= forceBetween;
        });

        velocity += effectiveForce * Time.deltaTime;

    }

    public void AddVelocity(Vector3 force)
    {
        velocity += force;
    }
}
using UnityEngine;
using System.Linq;
using System;

public class WorldElementReactor : MonoBehaviour
{
    private WorldElement worldElement;
    public WorldElement WorldElement
    {
        get
        {
            if (worldElement == null)
                worldElement = GetComponent<WorldElement>();
            if (worldElement == null)
                throw new ApplicationException("WorldElementReactor requires a WorldElement to work properly. Please add one first.");

            return worldElement;
        }
    }


    void Update()
    {

    }

    void UpdateNuclearForces()
    {
        var isNucleic = WorldElement.Data.Charge >= 0;

        if (!isNucleic) return;

        // Apply nuclear forces
        var worldMotors = Editor.SubElements;
        var otherWorldMotors = worldMotors.Where(x => x != this).ToList();

        var effectiveForce = Vector3.zero;
        otherWorldMotors.ForEach(otherElement =>
        {
            var forceBetween = WorldElement.ForceBetween(otherElement);

            // .. if nucleic particles (proton, neutron) are close enough, attract rather than repel
            // effectiveCharge = distanceToParticle < 2 && isNucleic ? -1 : effectiveCharge;

            effectiveForce += forceBetween;
        });
    }
}
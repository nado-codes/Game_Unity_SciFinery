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


    void Update()
    {

    }

    void UpdateNuclearForces()
    {
        var isNucleic = WorldElement.Data.Charge >= 0;

        if (!isNucleic) return;

        var worldMotors = Editor.SubElements;
        var otherWorldMotors = worldMotors.Where(x => x != this).ToList();

        otherWorldMotors.ForEach(otherElement =>
        {
            // .. if nucleic particles (proton, neutron) are close enough, attract rather than repel
            var distance = Vector3.Distance(otherElement.transform.position, transform.position);
            var bothNucleic = isNucleic && otherElement.Data.Charge >= 0;
            var useNuclear = distance < 2 && bothNucleic;

            if (useNuclear && !arcLight.Active)
                arcLight.SetActive(true);
            if (!useNuclear && arcLight.Active)
                arcLight.SetActive(false);

            var dirTo = (WorldElement.transform.position - otherElement.transform.position).normalized;
            arcLight.transform.position = dirTo;
        });
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("COLLIDED: " + col.transform.name);
    }
}
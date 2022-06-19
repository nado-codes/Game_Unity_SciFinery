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

    public bool IsFused = false;

    private AudioSource _popAudio;
    private AudioSource popAudio
    {
        get
        {
            if (_popAudio == null)
            {
                var audioSources = GetComponents<AudioSource>();
                _popAudio = audioSources.FirstOrDefault(a => a.clip.name == "Pop");
            }
            return _popAudio;
        }
    }


    void Update()
    {
        if (!IsFused)
            UpdateNuclearForces();
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
            var useNuclear = WorldUtilities.CanFuse(WorldElement, otherElement);

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
        var colWorldElement = col.gameObject.GetComponent<WorldElement>();
        var colReactor = col.gameObject.GetComponent<WorldElementReactor>();
        if (colWorldElement == null || IsFused) return;
        if (!WorldUtilities.CanFuse(WorldElement, colWorldElement)) return;
        IsFused = true;
        colReactor.IsFused = true;

        if (WorldElement.GetHashCode() > colWorldElement.GetHashCode())
            Editor.FuseElements(WorldElement, colWorldElement);

        // TODO: apply spin velocity to recently-fused particles 
        // - create a "ghost" particle that is pre-parented, for physics calculation
        // - collide both objects. object B will have correct spin applied
        // - remove the ghost, parent the object
        // - apply braking force to the particle group over time (lerp to 0)

        if (popAudio == null) return;
        popAudio.enabled = true;
        popAudio.Play();
    }
}
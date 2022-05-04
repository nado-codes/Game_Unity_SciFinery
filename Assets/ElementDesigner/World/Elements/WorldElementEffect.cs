using UnityEngine;
using System;
using System.Linq;

public class WorldElementEffect : MonoBehaviour
{
    private WorldElement worldElement;
    private bool worldElementIsRequired = false;
    private WorldElement WorldElement
    {
        get
        {
            if (worldElement == null)
                worldElement = GetComponent<WorldElement>();
            if (worldElement == null && worldElementIsRequired)
                throw new ApplicationException("WorldElementEffect requires a WorldElement to work properly. Please add one first.");

            return worldElement;
        }
    }

    private WorldElementMotor motor;
    private bool motorIsRequired = false;
    private WorldElementMotor Motor
    {
        get
        {
            if (motor == null)
                motor = GetComponent<WorldElementMotor>();
            if (motor == null && motorIsRequired)
                throw new ApplicationException("WorldElementEffect requires a WorldElementMotor to work properly. Please add one first.");

            return motor;
        }
    }

    public bool useParticles = false;
    private ParticleSystem particles;
    public bool useTrail = false;
    private TrailRenderer trail;
    public bool useSound = false;
    private AudioSource[] audioSources;

    private void VerifyInitialize()
    {
        particles = GetComponent<ParticleSystem>();

        trail = GetComponent<TrailRenderer>();
        trail.startWidth = WorldElement.BodyTransform.lossyScale.magnitude * .25f;

        audioSources = GetComponents<AudioSource>();

        if (!useSound)
            audioSources.ToList().ForEach(a => a.Stop());
    }

    void Start()
    {
        VerifyInitialize();
    }

    void Update()
    {
        worldElementIsRequired = useTrail;
        motorIsRequired = useTrail;

        if (useTrail)
        {
            trail.time = 10 * Mathf.Min((1 / Motor.Velocity.magnitude), 1f);

            trail.material.color = WorldElement.Color;
            trail.material.SetColor("_EmissionColor", WorldElement.Color);
        }
        else
            trail.time = 0;



    }
}
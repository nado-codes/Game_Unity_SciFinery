using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    private static List<Particle> particles = new List<Particle>();
    public static IEnumerable<Particle> Particles {
        get {
            if(particles.Count == 0)
                particles = FindObjectsOfType<Particle>().ToList();

            return particles;
        }
    }

    public static IEnumerable<Particle> OtherParticles(Particle particle) =>
        Particles.Where(x => x != particle);

    // Start is called before the first frame update
    void Start()
    {
        particles.AddRange(FindObjectsOfType<Particle>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddParticle(Particle particle)
    {
        if(!particles.Contains(particle))
            particles.Add(particle);

        Debug.Log("Adding particle: "+particle);
    }

    public static void RemoveParticle(Particle particle)
        => particles.Remove(particle);

    public static void RemoveParticles(IEnumerable<Particle> particlesToRemove)
        => particlesToRemove.Select(p => particles.Remove(p));

    public static void RemoveParticles(IEnumerable<Interact> particlesToRemove)
        => particlesToRemove.Select(p => particles.Remove(p.GetComponent<Particle>()));
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static List<Particle> particles = new List<Particle>();
    public static Particle[] Particles => particles.ToArray();

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
    {
        particles.Remove(particle);
    }
}

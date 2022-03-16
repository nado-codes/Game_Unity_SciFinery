using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proton : Particle
{
    // Start is called before the first frame update
    new void Start()
    {
        type = ParticleType.Proton;
        base.Start();
    }
}

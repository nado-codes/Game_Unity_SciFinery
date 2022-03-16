using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proton : WorldParticle
{
    // Start is called before the first frame update
    new void Start()
    {
        type = ParticleType.Proton;
        base.Start();
    }
}

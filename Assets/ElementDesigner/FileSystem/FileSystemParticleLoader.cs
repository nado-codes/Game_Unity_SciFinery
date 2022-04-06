using System.Collections.Generic;
using System.Linq;

public class FileSystemParticleLoader : FileSystemElementLoader
{
    public static IEnumerable<Particle> LoadParticles()
    {
        var protonParticle = new Particle()
        {
            Id = 1,
            Name = "Proton",
            Weight = .001f,
            Charge = 1,
            Size = 1,
            Type = ElementType.Particle,
            Color = "#00FFFA"
        };
        var neutronParticle = new Particle()
        {
            Id = 2,
            Name = "Neutron",
            Weight = 1,
            Charge = 0,
            Size = 1,
            Type = ElementType.Particle,
            Color = "#006F05"
        };
        var electronParticle = new Particle()
        {
            Id = 3,
            Name = "Electron",
            Weight = 10f,
            Charge = -1,
            Size = .5f,
            Type = ElementType.Particle,
            Color = "#FF0000"
        };

        var defaultParticles = new Particle[] { protonParticle, neutronParticle, electronParticle };
        return defaultParticles.Concat(loadElements<Particle>());
    }
}
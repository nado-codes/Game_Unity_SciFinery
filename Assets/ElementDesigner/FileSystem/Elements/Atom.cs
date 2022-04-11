using System;
using System.Linq;

public enum BondType { None = 0, Covalent = 1, Ionic = 2, Metallic = 3 }

[Serializable]
public class Atom : Element
{
    public Atom() : base(1)
    {
        Name = "NewAtom";
        ShortName = "NW";
        ParticleIds = new int[] { 1, 3 };
        ElementType = ElementType.Atom;
    }

    public int NeutralParticleCount = 0;

    public int[] ParticleIds = new int[0];

    // TODO: Valence shells and electrons divided into shells
    // TODO: Valence shell composition to affect reactivity

    public BondType BondType = BondType.None;

    public int Conductivity = 0;
    public int Reactivity = 0;
    public int Toxicity = 0;
    public int MeltingPoint = 0;
    public int BoilingPoint = 0;
    public int Brittleness = 0;
    public int Malleability = 0;
    public int Ductility = 0;

    public string Parent = "";
}

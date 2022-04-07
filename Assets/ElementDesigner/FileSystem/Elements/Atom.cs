using System;

public enum BondType { Covalent, Ionic, Metallic }

[Serializable]
public class Atom : Element
{
    public Atom(int id)
    {
        Id = id;
        Name = "NewAtom";
        ShortName = "NE";
        ParticleIds = new int[] { 1, 3 };
        Type = ElementType.Atom;
    }
    public int ProtonCount;
    public int NeutronCount;
    public int ElectronCount;

    public int[] ParticleIds;

    // TODO: Valence shells and electrons divided into shells
    // TODO: Valence shell composition to affect reactivity

    public BondType BondType;

    public int Conductivity;
    public int Reactivity;
    public int Toxicity;
    public int MeltingPoint;
    public int BoilingPoint;
    public int Brittleness;
    public int Malleability;
    public int Ductility;



    public bool IsIsotope { get; set; } = false;

    public int[] IsotopeAtomIds;
}

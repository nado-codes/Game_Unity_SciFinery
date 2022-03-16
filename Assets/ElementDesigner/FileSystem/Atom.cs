using System;

public enum BondType { Covalent, Ionic, Metallic }

[Serializable]
public class Atom : Element
{
    public int Number;

    public int ProtonCount;
    public int NeutronCount;
    public int ElectronCount;

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
}

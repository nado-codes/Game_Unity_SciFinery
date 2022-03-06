using System;

public enum BondType { Covalent, Ionic, Metallic }

[Serializable]
public class Atom : Element
{
    public int ProtonCount;
    public int NeutronCount;
    public int ElectronCount;
    public float Charge;
    public int Number;

    // TODO: Valence shells and electrons divided into shells
    // TODO: Valence shell composition to affect reactivity

    public BondType BondType;



    public bool IsIsotope { get; set; } = false;
}

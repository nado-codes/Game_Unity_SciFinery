using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BondType {Covalent, Ionic, Metallic}

[Serializable]
public class Atom
{
    public int ProtonCount;
    public int NeutronCount;
    public int ElectronCount;

    public int Charge;

    // TODO: Valence shells and electrons divided into shells
    // TODO: Valence shell composition to affect reactivity

    public BondType BondType;
    public int Number;
    public string ShortName;
    public string Name;
    public int Weight;
    public int Conductivity;
    public int Reactivity;
    public int Toxicity;
    public int MeltingPoint;
    public int BoilingPoint;
    public int Brittleness;
    public int Malleability;
    public int Ductility;
}

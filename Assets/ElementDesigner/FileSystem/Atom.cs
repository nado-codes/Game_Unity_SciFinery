using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BondType {Covalent, Ionic, Metallic}

[Serializable]
public class Atom : Element
{
    public int ProtonCount;
    public int NeutronCount;
    public int ElectronCount;
    public float Charge;

    public int Number;
    public string ShortName;
    public string Name;

    // TODO: Valence shells and electrons divided into shells
    // TODO: Valence shell composition to affect reactivity

    public BondType BondType;
    
    public int Weight;
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

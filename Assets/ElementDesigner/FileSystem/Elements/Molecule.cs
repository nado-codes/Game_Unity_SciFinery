using System;
using UnityEngine;

[Serializable]
public class Molecule : Element
{
    public Molecule() : base(1)
    {
        Name = "NewMolecule";
        ShortName = "NW";
        AtomIds = new int[0];
        ElementType = ElementType.Molecule;
    }
    public int[] AtomIds = new int[0];
}


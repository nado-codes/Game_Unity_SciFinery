using System;
using UnityEngine;

[Serializable]
public class Molecule : Element
{
    public Molecule() : base(1)
    {
        Name = "NewMolecule";
        AtomIds = new int[0];
        ElementType = ElementType.Molecule;
        SubElementType = ElementType.Atom;
    }
    public int[] AtomIds = new int[0];
}


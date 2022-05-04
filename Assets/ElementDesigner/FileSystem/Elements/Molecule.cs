using System;
using UnityEngine;

[Serializable]
public class Molecule : Element
{
    public Molecule()
    {
        Name = "NewMolecule";
        ElementType = ElementType.Molecule;
        SubElementType = ElementType.Atom;
    }
}


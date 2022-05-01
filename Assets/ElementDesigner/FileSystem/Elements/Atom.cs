using System;
using System.Collections.Generic;
using System.Linq;

public enum BondType { None = 0, Covalent = 1, Ionic = 2, Metallic = 3 }

[Serializable]
public class Atom : Element
{
    public Atom()
    {
        Name = "NewAtom";
        ChildIds = new int[] { 1, 3 };
        ElementType = ElementType.Atom;
        SubElementType = ElementType.Particle;
    }

    ///<summary>The atomic number of this atom, based on Proton count</summary>
    public int Number => Children.Count(ch => ch.Charge > 0);

    public List<Particle> Particles
    {
        get
        {
            if (base.Children.Any(c => c.ElementType != ElementType.Particle))
                throw new ApplicationException("All Atom children must be Particles");

            return base.Children.Cast<Particle>().ToList();
        }
    }
    public int[] IsotopeIds = new int[0];
    public List<Atom> Isotopes
        => FileSystemCache.GetOrLoadElementsOfTypeByIds<Atom>(IsotopeIds).ToList();
    public int ParentId = -1;
    public Atom Parent => FileSystemCache.GetOrLoadElementOfTypeById<Atom>(ParentId);

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


}


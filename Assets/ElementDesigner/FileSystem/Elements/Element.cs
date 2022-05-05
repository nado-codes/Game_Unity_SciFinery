using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

///<summary>[Data] Master class. Any object which may exist in world space</summary>
[Serializable]
public class Element
{
    public Element() { }

    public int Id = -1;
    ///<summary>A shorthand abbreviated version of [Name] e.g. Hydrogen->HY</summary>
    public string ShortName =>
        string.Join("", Name.Substring(0, 2).Select((c, i) => i == 0 ? c.ToString().ToUpper() : c.ToString().ToLower()
    ));
    public string Name;
    ///<summary>How physically heavy an element is. Also partially determines how other elements react to it</summary>
    private float weight;
    public float Weight
    {
        get
        {
            if (ElementType == ElementType.Particle)
                return weight;
            else
                return Children.Any() ? Children.Select(c => c.Weight).Aggregate((a, c) => a + c) : 0;
        }
        set
        {
            if (ElementType == ElementType.Particle)
                weight = value;
        }
    }
    ///<summary>Whether this element attracts or repulses other elements. Works together with Weight to determine overall repulsive/attractive force</summary>
    private int charge = 0;
    public virtual int Charge
    {
        get
        {
            if (ElementType == ElementType.Particle)
                return charge;

            return Convert.ToInt32(Children.Sum(c => c.Charge));
        }
        set
        {
            if (ElementType == ElementType.Particle)
                charge = value;
            else
                throw new ApplicationException("Only Particles (base element type) may have a preset charge");
        }
    }
    public ElementType ElementType = ElementType.Atom;
    public ElementType SubElementType = ElementType.Particle;

    public int[] ChildIds = new int[0];

    private List<Element> children = new List<Element>();
    public List<Element> Children
    {
        get
        {
            children = FileSystemCache.GetOrLoadSubElementsOfTypeByIds(SubElementType, ChildIds).ToList();
            return children;
        }
    }

    ///<summary>Hexadecimal value representing the baseColor of a particle in world space</summary>
    public Color Color => Utilities.BlendColors(children.Select(c => c.Color).ToArray());
}
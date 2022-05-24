using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

///<summary>[Data] Master class. Any object which may exist in world space</summary>
[Serializable]
public class Element
{
    public Element() { }

    ///<summary>The unique id of this element in the element cache</summary>
    public int Id = -1;
    ///<summary>Whether or not this element is visible or usable. Allows dependent elements to keep referencing it.</summary>
    public bool IsDeleted = false;
    ///<summary>A shorthand abbreviated version of [Name] e.g. Hydrogen->HY</summary>
    public string ShortName =>
        string.Join("", Name.Substring(0, 2).Select((c, i) => i == 0 ? c.ToString().ToUpper() : c.ToString().ToLower()
    ));
    public string Name;

    private float weight;
    ///<summary>How physically heavy an element is. Also partially determines how other elements react to it</summary>
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

    private int charge = 0;
    ///<summary>Whether this element attracts or repulses other elements. Works together with Weight to determine overall repulsive/attractive force</summary>
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

    ///<summary>List of Ids for SubElements attached to this element. Actual type depends on SubElementType</summary>
    public int[] ChildIds = new int[0];

    [NonSerialized]
    private List<Element> children = new List<Element>();

    ///<summary>List of SubElements attached to this element</summary> //
    public List<Element> Children
    {
        get
        {
            children = FileSystemCache.GetOrLoadSubElementsOfTypeByIds(SubElementType, ChildIds).ToList();
            return children;
        }
    }

    [SerializeField]
    private Color defaultColor = Color.white;
    ///<summary>Hexadecimal value representing the color of an element in world space</summary>
    public string ColorHex
    {
        get => "#" + ColorUtility.ToHtmlStringRGBA(Color);
        set
        {
            ColorUtility.TryParseHtmlString(value, out Color newColor);
            defaultColor = newColor;
        }
    }

    ///<summary>RGBA value representing the the color of an element in world space</summary>
    public Color Color => children.Any() ? Utilities.BlendColors(children.Select(c => c.Color).ToArray()) : defaultColor;
}
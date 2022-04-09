using System;

///<summary>[Data] Master class. Any object which may exist in world space</summary>
[Serializable]
public class Element
{
    public Element(int id)
    {
        Id = id;
    }

    public Element Copy()
    {

    }
    public int Id;

    ///<summary>A shorthand abbreviated version of [Name] e.g. Hydrogen->HY</summary>
    public string ShortName;
    public string Name;
    ///<summary>How physically heavy an element is. Also partially determines how other elements react to it</summary>
    public float Weight;
    ///<summary>Whether this element attracts or repulses other elements. Works together with Weight to determine overall repulsive/attractive force</summary>
    public float Charge;
    public ElementType Type;
}
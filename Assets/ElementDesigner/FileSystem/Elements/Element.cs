using System;
using System.Linq;

///<summary>[Data] Master class. Any object which may exist in world space</summary>
[Serializable]
public class Element
{
    public Element() { }
    public Element(int id)
    {
        Id = id;
    }
    public void AddChild(Element element)
    {
        Array.Resize(ref Children, Children.Length + 1);

        var newElementIndex = Children.Length;
        Children[newElementIndex] = element.Id;
    }

    public void RemoveChildById(int idToRemove)
    {
        if (!Children.Any(id => id == idToRemove))
            throw new NullReferenceException($"Element with id {idToRemove} isn't a child of the parent element");

        Children = Children.Where(id => id != idToRemove).ToArray();
    }

    public int Id;
    ///<summary>A shorthand abbreviated version of [Name] e.g. Hydrogen->HY</summary>
    public string ShortName;
    public string Name;
    ///<summary>How physically heavy an element is. Also partially determines how other elements react to it</summary>
    public float Weight;
    ///<summary>Whether this element attracts or repulses other elements. Works together with Weight to determine overall repulsive/attractive force</summary>
    public float Charge;
    public ElementType ElementType;

    public int[] Children = new int[0];
}
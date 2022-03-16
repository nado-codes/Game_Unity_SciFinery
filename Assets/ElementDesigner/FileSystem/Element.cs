using System;

[Serializable]
public class Element
{
    public string Guid;

    public string ShortName;
    public string Name;
    public int Weight;
    public int Charge;
    public ElementType Type;


    // TODO: elements should have a list or dictionary for their properties, so all the properties above will be serialized/deserialized
    // seperately when saving/loading
}
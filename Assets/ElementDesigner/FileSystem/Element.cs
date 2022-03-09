using System;

[Serializable]
public class Element
{
    public string Guid;

    public string ShortName;
    public string Name;
    public int Weight;
    public int Conductivity;
    public int Reactivity;
    public int Toxicity;
    public int MeltingPoint;
    public int BoilingPoint;
    public int Brittleness;
    public int Malleability;
    public int Ductility;

    // TODO: elements should have a list or dictionary for their properties, so all the properties above will be serialized/deserialized
    // seperately when saving/loading
}